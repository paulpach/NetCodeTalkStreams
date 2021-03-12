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
using System.Threading;

namespace Transport {
  class TestPeer {
    public const ushort SERVER_PORT = 25005;

    public const int NUMBER_COUNT = 16;

    public static IPEndPoint ServerEndPoint => new IPEndPoint(IPAddress.Loopback, SERVER_PORT);

    bool _server;

    public Peer Peer;

    public bool IsServer => _server;
    public bool IsClient => _server == false;

    Connection _remote;

    int _numberCounter;

    public TestPeer(bool server) {
      _server = server;

      Peer                    =  new Peer(GetConfig(server));
      Peer.OnConnected        += PeerOnConnected;
      Peer.OnUnreliablePacket += PeerOnUnreliablePacket;

      Peer.OnNotifyPacketLost      += PeerOnNotifyPacketLost;
      Peer.OnNotifyPacketDelivered += PeerOnNotifyPacketDelivered;

      if (IsClient) {
        Peer.Connect(ServerEndPoint);
      }
    }

    void PeerOnNotifyPacketLost(Connection arg1, object lost) {
      if (lost != null) {
        Log.Info($"Resend: {lost}");
        Peer.SendNotify(_remote, BitConverter.GetBytes((int) lost), lost);
      }
    }

    void PeerOnNotifyPacketDelivered(Connection arg1, object delivered) {
      if (delivered != null) {
        Log.Info($"Delivered: {delivered}");
      }
    }

    void PeerOnUnreliablePacket(Connection connection, Packet packet) {
      Log.Info($"Got Data: {BitConverter.ToUInt32(packet.Data, packet.Offset)}");
    }

    void PeerOnConnected(Connection connection) {
      _remote = connection;
    }

    public static Config GetConfig(bool server) {
      Config config;
      config               = new Config();
      config.SimulatedLoss = 0.25;

      if (server) {
        config.EndPoint = ServerEndPoint;
      } else {
        config.EndPoint = new IPEndPoint(IPAddress.Any, 0);
      }

      return config;
    }

    public void Update() {
      Peer?.Update();

      if (_remote != null) {
        if (IsClient && _numberCounter < NUMBER_COUNT) {
          Peer.SendNotify(_remote, BitConverter.GetBytes(++_numberCounter), _numberCounter);
        } else {
          Peer.SendNotify(_remote, new byte[0], null);
        }
      }
    }
  }

  static class Program {
    static List<TestPeer> Peers = new List<TestPeer>();

    static void Main(string[] args) {
      Log.InitForConsole();

      //Sequencer sequencer = new Sequencer(1);
      // ... 254 255 0 1 2 3 ... 
      //Log.Info(sequencer.Distance(255, 0));

      Peers.Add(new TestPeer(true));
      Peers.Add(new TestPeer(false));

      while (true) {
        for (int i = 0; i < Peers.Count; ++i) {
          Peers[i].Update();
        }

        Thread.Sleep(100);
      }

      Console.ReadLine();
    }
  }
}