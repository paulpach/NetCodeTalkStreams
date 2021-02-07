using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// 1) dynamically adjusting interpolation offset
// 2) how to do prediction

public struct FloatIntegratorEma {
  float _ratio;
  float _average;

  bool _first;

  public bool Initialized {
    get { return _ratio != 0f; }
  }

  public float Value {
    get { return _average; }
  }

  public void Initialize(int count) {
    this   = default;
    _ratio = 2.0f / (count + 1);
    _first = true;
  }

  public void Integrate(float value) {
    if (_first) {
      _average = value;
      _first   = false;
    } else {
      _average += _ratio * (value - _average);
    }
  }
}

public class SnapshotInterpolation : MonoBehaviour {
  struct Snapshot {
    public Vector3 Position;
    public float   Time;
    public float   DeliveryTime;
  }

  public GameObject Server;
  public GameObject Client;

  float _lastSnapshot;

  FloatIntegratorEma _clientTimeOffsetAvg;
  
  FloatIntegratorEma _clientSnapshotDeliveryDeltaAvg;
  float?             _clientLastSnapshotReceived;

  float           _clientMaxServerTimeReceived;
  float           _clientInterpolationTime;
  float           _clientInterpolationTimeScale;
  Queue<Snapshot> _clientNetworkSimulationQueue = new Queue<Snapshot>();
  List<Snapshot>  _clientSnapshots              = new List<Snapshot>();

  const int   SNAPSHOT_RATE         = 30;
  const float SNAPSHOT_INTERVAL     = 1.0f / SNAPSHOT_RATE;
  const int   SNAPSHOT_OFFSET_COUNT = 2;
  const float INTERPOLATION_OFFSET  = SNAPSHOT_INTERVAL * SNAPSHOT_OFFSET_COUNT;

  const float INTERPOLATION_TIME_ADJUSTMENT_NEGATIVE_THRESHOLD = SNAPSHOT_INTERVAL * -0.5f;
  const float INTERPOLATION_TIME_ADJUSTMENT_POSITIVE_THRESHOLD = SNAPSHOT_INTERVAL * 2;

  void Start() {
    _clientInterpolationTimeScale = 1;
  }

  void Update() {
    // server logic
    ServerMovement();
    ServerSnapshot();

    // client logic
    ClientUpdateInterpolationTime();
    ClientReceiveDataFromServer();
    ClientRenderLatestPostion();

    // moving avg integrator to track client offset vs server
    _clientTimeOffsetAvg = new FloatIntegratorEma();
    _clientTimeOffsetAvg.Initialize(SNAPSHOT_RATE);

    _clientSnapshotDeliveryDeltaAvg = new FloatIntegratorEma();
    _clientSnapshotDeliveryDeltaAvg.Initialize(SNAPSHOT_RATE);
  }

  void ClientReceiveDataFromServer() {
    var received = false;

    while (_clientNetworkSimulationQueue.Count > 0 && _clientNetworkSimulationQueue.Peek().DeliveryTime < Time.time) {
      // this is our first snapshot
      if (_clientSnapshots.Count == 0) {
        _clientInterpolationTime = _clientNetworkSimulationQueue.Peek().Time - INTERPOLATION_OFFSET;
      }

      var snapshot = _clientNetworkSimulationQueue.Dequeue();

      _clientSnapshots.Add(snapshot);
      _clientMaxServerTimeReceived = Math.Max(_clientMaxServerTimeReceived, snapshot.Time);

      received = true;
    }

    if (received) {
      if (_clientLastSnapshotReceived.HasValue) {
        _clientSnapshotDeliveryDeltaAvg.Integrate(Time.time - _clientLastSnapshotReceived.Value);
      }

      // for next time we receive a snapshot
      _clientLastSnapshotReceived = Time.time;
      
      // this is the difference between latest time we've received from the server and our local interpolation time 
      var diff = _clientMaxServerTimeReceived - _clientInterpolationTime;

      // 
      _clientTimeOffsetAvg.Integrate(diff);

      // this is the difference between our current time offset and the wanted interpolation offset
      var diffWanted = _clientTimeOffsetAvg.Value - INTERPOLATION_OFFSET;

      // if diffWanted is positive it means that we are *a head* of where we want to be (i.e. more offset than needed) 
      if (diffWanted > INTERPOLATION_TIME_ADJUSTMENT_POSITIVE_THRESHOLD) {
        _clientInterpolationTimeScale = 1.01f;
      } else if (diffWanted < INTERPOLATION_TIME_ADJUSTMENT_NEGATIVE_THRESHOLD) {
        _clientInterpolationTimeScale = 0.99f;
      } else {
        _clientInterpolationTimeScale = 1.0f;
      }
      
      Debug.Log($"diff: {diff:F3}, diffWanted: {diffWanted:F3}, timeScale:{_clientInterpolationTimeScale:F3}, deliveryDeltaAvg:{_clientSnapshotDeliveryDeltaAvg.Value}");
    }
  }

  void ClientUpdateInterpolationTime() {
    if (_clientSnapshots.Count > 0) {
      _clientInterpolationTime += (Time.unscaledDeltaTime * _clientInterpolationTimeScale);
    }
  }

  void ClientRenderLatestPostion() {
    if (_clientSnapshots.Count > 0) {
      var interpFrom  = default(Vector3);
      var interpTo    = default(Vector3);
      var interpAlpha = default(float);

      for (int i = 0; i < _clientSnapshots.Count; ++i) {
        if (i + 1 == _clientSnapshots.Count) {
          if (_clientSnapshots[0].Time > _clientInterpolationTime) {
            interpFrom  = interpTo = _clientSnapshots[0].Position;
            interpAlpha = 0;
          } else {
            interpFrom  = interpTo = _clientSnapshots[i].Position;
            interpAlpha = 0;
          }
        } else {
          //                v----- client interp time
          // [0][1][2][3][4] [5][6][7][8][9]
          //              F
          //                 T

          // F = 101.4 seconds
          // INTERP TIME = 101.467
          // T = 101.5 seconds

          var f = i;
          var t = i + 1;

          if (_clientSnapshots[f].Time <= _clientInterpolationTime && _clientSnapshots[t].Time >= _clientInterpolationTime) {
            interpFrom = _clientSnapshots[f].Position;
            interpTo   = _clientSnapshots[t].Position;

            var range   = _clientSnapshots[t].Time - _clientSnapshots[f].Time;
            var current = _clientInterpolationTime - _clientSnapshots[f].Time;

            interpAlpha = Mathf.Clamp01(current / range);

            break;
          }
        }
      }

      Client.transform.position = Vector3.Lerp(interpFrom, interpTo, interpAlpha);
    }
  }

  void ServerMovement() {
    Vector3 pos;
    pos   = default;
    pos.x = Mathf.PingPong(Time.time * 5, 10f) - 5f;

    Server.transform.position = pos;
  }

  void ServerSnapshot() {
    if (_lastSnapshot + SNAPSHOT_INTERVAL < Time.time) {
      _lastSnapshot = Time.time;
      _clientNetworkSimulationQueue.Enqueue(new Snapshot {
        Time         = _lastSnapshot,
        Position     = Server.transform.position,
        DeliveryTime = Time.time + (Random.value * 0.05f)
      });
    }
  }
}