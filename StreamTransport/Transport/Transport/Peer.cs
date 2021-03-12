/*
The MIT License (MIT)

Copyright (c) 2020 Fredrik Holmstrom

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Transport {
  public class Peer {
    Timer  _clock;
    Socket _socket;
    Config _config;
    Random _random;

    Dictionary<IPEndPoint, Connection> _connections;

    const int ACK_MASK_BITS = sizeof(ulong) * 8;

    int NotifyPacketHeaderSize => 9 + (_config.SequenceNumberBytes * 2);
    
    public Config Config => _config;

    public event Action<Connection, ConnectionFailedReason> OnConnectionFailed;
    public event Action<Connection>                         OnConnected;
    public event Action<Connection, DisconnectedReason>     OnDisconnected;
    public event Action<Connection, Packet>                 OnUnreliablePacket;
    
    public event Action<Connection, object> OnNotifyPacketLost;
    public event Action<Connection, object> OnNotifyPacketDelivered;
    public event Action<Connection, Packet> OnNotifyPacketReceived;

    public Peer(Config config) {
      _config = config;
      _clock  = Timer.StartNew();
      _random = new Random(Environment.TickCount);

      _connections = new Dictionary<IPEndPoint, Connection>();

      _socket          = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      _socket.Blocking = false;
      _socket.Bind(config.EndPoint);

      Log.Info($"Socket Bound To {_socket.LocalEndPoint}");

      SetConnReset(_socket);
    }

    public void SendUnreliable(Connection connection, byte[] data) {
      if (data.Length > (_config.Mtu - 1)) {
        Log.Error($"Data to large, above MTU-1: {data.Length}");
        return;
      }

      var buffer = GetMtuBuffer();
      Buffer.BlockCopy(data, 0, buffer, 1, data.Length);
      buffer[0] = (byte) PacketTypes.Unreliable;

      Send(connection, buffer, data.Length + 1);
    }

    public bool SendNotify(Connection connection, byte[] data, object userObject) {
      if (connection.SendWindow.IsFull) {
        return false;
      }

      // 1: packet type enum                      - 1 byte
      // 2: sequence number for this packet       - _config.SequenceNumberBytes bytes
      // 3: recv sequence                         - _config.SequenceNumberBytes bytes
      // 4: recv mask                             - 8 bytes

      if (data.Length > (_config.Mtu - NotifyPacketHeaderSize)) {
        throw new InvalidOperationException();
      }

      var sequenceNumberForPacket = connection.SendSequencer.Next();

      var buffer = GetMtuBuffer();

      // copy from data into buffer
      Buffer.BlockCopy(data, 0, buffer, NotifyPacketHeaderSize, data.Length);

      // write packet type
      buffer[0] = (byte) PacketTypes.Notify;
      var offset = 1;

      // write sequence number of *this* packet
      ByteUtils.WriteULong(buffer, offset, _config.SequenceNumberBytes, sequenceNumberForPacket);
      offset += _config.SequenceNumberBytes;

      // write *recv sequence* for this connection
      ByteUtils.WriteULong(buffer, offset, _config.SequenceNumberBytes, connection.RecvSequence);
      offset += _config.SequenceNumberBytes;

      // write *recv mask* for this connection
      ByteUtils.WriteULong(buffer, offset, 8, connection.RecvMask);

      // push on send window
      connection.SendWindow.Push(new SendEnvelope {
        Sequence = sequenceNumberForPacket,
        SendTime     = _clock.ElapsedInSeconds,
        UserData = userObject
      });

      Send(connection, buffer, NotifyPacketHeaderSize + data.Length);
      return true;
    }

    void HandleNotifyPacket(Connection connection, Packet packet) {
      if (packet.Length < NotifyPacketHeaderSize) {
        return;
      }
      
      packet.Offset = 1;

      var packetSequenceNumber = ByteUtils.ReadUlong(packet.Data, packet.Offset, _config.SequenceNumberBytes);
      packet.Offset += _config.SequenceNumberBytes;

      var remoteRecvSequence = ByteUtils.ReadUlong(packet.Data, packet.Offset, _config.SequenceNumberBytes);
      packet.Offset += _config.SequenceNumberBytes;

      var remoteRecvMask = ByteUtils.ReadUlong(packet.Data, packet.Offset, 8);
      packet.Offset += 8;

      var sequenceDistance = connection.SendSequencer.Distance(packetSequenceNumber, connection.RecvSequence);

      // sequence is so far out of bounds we can't save, just kick him (or her!)
      if (Math.Abs(sequenceDistance) > _config.SendWindowSize) { 
        DisconnectConnection(connection, DisconnectedReason.SequenceOutOfBounds);
        return;
      }

      // sequence is old, so duplicate or re-ordered packet
      if (sequenceDistance <= 0) {
        return;
      }
      
      // ((sequence_number_bytes * 8) + (ack_mask_bits)): 16 + 64 = 1.25bits

      // update recv sequence for our local connection object
      connection.RecvSequence = packetSequenceNumber;

      if (sequenceDistance >= ACK_MASK_BITS) {
        connection.RecvMask = 1; // 0000 0000 0000 0000 0000 0000 0000 0001;
      } else {
        connection.RecvMask = (connection.RecvMask << (int)sequenceDistance) | 1;
      }

      // handle acking
      AckPackets(connection, remoteRecvSequence, remoteRecvMask);
      
      // call out to userland!
      OnNotifyPacketReceived?.Invoke(connection, packet);
    }

    void AckPackets(Connection connection, ulong recvSequenceFromRemote, ulong recvMaskFromRemote) {
      while (connection.SendWindow.Count > 0) {
        var envelope = connection.SendWindow.Peek();

        var distance = (int)connection.SendSequencer.Distance(envelope.Sequence, recvSequenceFromRemote);
        if (distance > 0) {
          break;
        }

        // remove envelope from send window
        connection.SendWindow.Pop();
        
        // if this is the same as the latest sequence remote received from us, we can use this to calculate RTT 
        if (distance == 0) {
          connection.Rtt = _clock.ElapsedInSeconds - envelope.SendTime;
        }

        // if any of these cases trigger, packet is most likely lost
        if ((distance <= -ACK_MASK_BITS) || ((recvMaskFromRemote & (1UL << -distance)) == 0UL)) {
          OnNotifyPacketLost?.Invoke(connection, envelope.UserData);
        } else {
          OnNotifyPacketDelivered?.Invoke(connection, envelope.UserData);
        }
      }
    }

    public void Disconnect(Connection connection) {
      if (connection.State != ConnectionState.Connected) {
        Log.Error($"Can't disconnect {connection} state is {connection.State}");
        return;
      }

      DisconnectConnection(connection, DisconnectedReason.RequestedByPeer);
    }

    public void Connect(IPEndPoint endpoint) {
      CreateConnection(endpoint).ChangeState(ConnectionState.Connecting);
    }

    public void Update() {
      // recv
      Recv();

      // process
      UpdateConnections();
    }

    public void SendUnconnected(EndPoint target, byte[] data) {
      _socket.SendTo(data, target);
    }

    void UpdateConnections() {
      foreach (var kvp in _connections) {
        UpdateConnection(kvp.Value);
      }
    }

    void UpdateConnection(Connection connection) {
      switch (connection.State) {
        case ConnectionState.Connecting:
          UpdateConnecting(connection);
          break;

        case ConnectionState.Connected:
          UpdateConnected(connection);
          break;

        case ConnectionState.Disconnected:
          UpdateDisconnected(connection);
          break;
      }
    }

    void UpdateDisconnected(Connection connection) {
      if ((connection.DisconnectTime + _config.DisconnectIdleTime) < _clock.ElapsedInSeconds) {
        RemoveConnection(connection);
      }
    }

    void UpdateConnected(Connection connection) {
      if ((connection.LastRecvPacketTime + _config.ConnectionTimeout) < _clock.ElapsedInSeconds) {
        DisconnectConnection(connection, DisconnectedReason.Timeout);
      }

      if ((connection.LastSentPacketTime + _config.KeepAliveInterval) < _clock.ElapsedInSeconds) {
        Send(connection, new byte[1] {(byte) PacketTypes.KeepAlive});
      }
    }

    void DisconnectConnection(Connection connection, DisconnectedReason reason, bool sendToOtherPeer = true) {
      if (sendToOtherPeer) {
        SendCommand(connection, Commands.Disconnect, (byte) reason);
      }

      connection.ChangeState(ConnectionState.Disconnected);
      connection.DisconnectTime = _clock.ElapsedInSeconds;
      OnDisconnected?.Invoke(connection, reason);
    }

    void UpdateConnecting(Connection connection) {
      if ((connection.ConnectionAttemptTime + _config.ConnectAttemptInterval) < _clock.ElapsedInSeconds) {
        if (connection.ConnectionAttempts == _config.MaxConnectAttempts) {
          Assert.AlwaysFail("connection failed handle this with a callback");
          return;
        }

        connection.ConnectionAttempts    += 1;
        connection.ConnectionAttemptTime =  _clock.ElapsedInSeconds;

        SendCommand(connection, Commands.ConnectRequest);
      }
    }

    byte[] GetMtuBuffer() {
      return new byte[_config.Mtu];
    }

    void Recv() {
      while (_socket.Poll(0, SelectMode.SelectRead)) {
        var buffer        = GetMtuBuffer();
        var endpoint      = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
        var bytesReceived = _socket.ReceiveFrom(buffer, SocketFlags.None, ref endpoint);
        if (bytesReceived > 0) {
          
          // simulate loss
          if (_config.SimulatedLoss > 0) {
            
            // 0.0-0.1
            if (_random.NextDouble() < _config.SimulatedLoss) {
              //Log.Trace($"Simulated Loss Of {bytesReceived} Bytes From {endpoint}");
              return;
            }
            
          }

          var packet = new Packet {
            Data   = buffer,
            Length = bytesReceived
          };

          //Log.Trace($"Received {bytesReceived} Bytes From {endpoint}");

          var ipEndpoint = (IPEndPoint) endpoint;

          if (_connections.TryGetValue(ipEndpoint, out var connection)) {
            if (connection.State != ConnectionState.Disconnected) {
              HandleConnectedPacket(connection, packet);
            }
          } else {
            HandleUnconnectedPacket(ipEndpoint, packet);
          }
        }
      }
    }

    void HandleConnectedPacket(Connection connection, Packet packet) {
      connection.LastRecvPacketTime = _clock.ElapsedInSeconds;

      switch ((PacketTypes) packet.Data[0]) {
        case PacketTypes.Command:
          HandleCommandPacket(connection, packet);
          break;

        case PacketTypes.Unreliable:
          HandleUnreliablePacket(connection, packet);
          break;

        case PacketTypes.KeepAlive:
          // do nothing
          break;

        case PacketTypes.Notify:
          HandleNotifyPacket(connection, packet);
          break;
      }
    }


    void HandleUnreliablePacket(Connection connection, Packet packet) {
      packet.Offset = 1;
      OnUnreliablePacket?.Invoke(connection, packet);
    }

    void HandleUnconnectedPacket(IPEndPoint endpoint, Packet packet) {
      // assume is garbage data from somewhere on the internet
      if (packet.Length != 2) {
        return;
      }

      // first packet has to be a command
      if (((PacketTypes) packet.Data[0]) != PacketTypes.Command) {
        return;
      }

      // first packet has to be a connect request command
      if (((Commands) packet.Data[1]) != Commands.ConnectRequest) {
        return;
      }

      // we know packet is valid ... but we don't have enough connection slots
      if (_connections.Count >= _config.MaxConnections) {
        SendUnconnected(endpoint, new byte[3] {
          (byte) PacketTypes.Command,
          (byte) Commands.ConnectionRefused,
          (byte) ConnectionFailedReason.ServerFull
        });

        return;
      }

      // 
      HandleCommandPacket(CreateConnection(endpoint), packet);
    }

    Connection CreateConnection(IPEndPoint endpoint) {
      Connection connection;

      connection                    = new Connection(_config, endpoint);
      connection.LastRecvPacketTime = _clock.ElapsedInSeconds;

      _connections.Add(endpoint, connection);

      Log.Trace($"created connection object for {endpoint}");

      return connection;
    }

    void RemoveConnection(Connection connection) {
      Assert.Check(connection.State != ConnectionState.Destroyed);
      var removed = _connections.Remove(connection.RemoteEndPoint);
      connection.ChangeState(ConnectionState.Destroyed);
      Assert.Check(removed);
    }

    void HandleCommandPacket(Connection connection, Packet packet) {
      Log.Trace($"Received Command {(Commands) packet.Data[1]} From {connection}");

      switch ((Commands) packet.Data[1]) {
        case Commands.ConnectRequest:
          HandleConnectRequest(connection, packet);
          break;

        case Commands.ConnectionAccepted:
          HandleConnectionAccepted(connection, packet);
          break;

        case Commands.ConnectionRefused:
          HandleConnectionRefused(connection, packet);
          break;

        case Commands.Disconnect:
          HandleDisconnected(connection, packet);
          break;

        default:
          Log.Info($"Unknown Command: {(Commands) packet.Data[1]}");
          break;
      }
    }

    void HandleDisconnected(Connection connection, Packet packet) {
      DisconnectConnection(connection, (DisconnectedReason) packet.Data[3], false);
    }

    void HandleConnectionRefused(Connection connection, Packet packet) {
      switch (connection.State) {
        case ConnectionState.Connecting:
          var reason = (ConnectionFailedReason) packet.Data[2];
          Log.Trace($"Connection Refused: {reason}");
          RemoveConnection(connection);
          OnConnectionFailed?.Invoke(connection, reason);
          break;

        default:
          Assert.AlwaysFail();
          break;
      }
    }

    void HandleConnectRequest(Connection connection, Packet packet) {
      switch (connection.State) {
        case ConnectionState.Created:
          SetConnectionAsConnected(connection);
          SendCommand(connection, Commands.ConnectionAccepted);
          break;

        case ConnectionState.Connected:
          SendCommand(connection, Commands.ConnectionAccepted);
          break;

        case ConnectionState.Connecting:
          Assert.AlwaysFail();
          break;
      }
    }

    void HandleConnectionAccepted(Connection connection, Packet packet) {
      switch (connection.State) {
        case ConnectionState.Created:
          Assert.AlwaysFail();
          break;

        case ConnectionState.Connected:
          // ignore this
          break;

        case ConnectionState.Connecting:
          SetConnectionAsConnected(connection);
          break;
      }
    }

    void SetConnectionAsConnected(Connection connection) {
      connection.ChangeState(ConnectionState.Connected);
      OnConnected?.Invoke(connection);
    }

    void SendCommand(Connection connection, Commands command, byte? commandData = null) {
      Assert.Check(connection.State < ConnectionState.Disconnected);

      Log.Trace($"Sending Command {command} To {connection}");

      if (commandData.HasValue) {
        Send(connection, new byte[3] {(byte) PacketTypes.Command, (byte) command, commandData.Value});
      } else {
        Send(connection, new byte[2] {(byte) PacketTypes.Command, (byte) command});
      }
    }

    void Send(Connection connection, byte[] data, int? length = null) {
      Assert.Check(connection.State < ConnectionState.Disconnected);

      connection.LastSentPacketTime = _clock.ElapsedInSeconds;

      if (length.HasValue) {
        _socket.SendTo(data, 0, length.Value, SocketFlags.None, connection.RemoteEndPoint);
      } else {
        _socket.SendTo(data, connection.RemoteEndPoint);
      }
    }


    static void SetConnReset(Socket s) {
      try {
        const uint IOC_IN     = 0x80000000;
        const uint IOC_VENDOR = 0x18000000;
        s.IOControl(unchecked((int) (IOC_IN | IOC_VENDOR | 12)), new byte[] {System.Convert.ToByte(false)}, null);
      } catch {
      }
    }
  }
}