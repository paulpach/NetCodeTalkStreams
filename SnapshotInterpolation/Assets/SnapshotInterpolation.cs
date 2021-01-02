using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1) dynamically adjusting interpolation offset
// 2) how to do prediction
// 3) how to adjust client time if it drifts from server

public class SnapshotInterpolation : MonoBehaviour {
  struct Snapshot {
    public Vector3 Position;
    public float   Time;
    public float   DeliveryTime;
  }

  public GameObject Server;
  public GameObject Client;

  float _lastSnapshot;

  float           _clientInterpolationTime;
  Queue<Snapshot> _clientNetworkSimulationQueue = new Queue<Snapshot>();
  List<Snapshot>  _clientSnapshots              = new List<Snapshot>();

  const float SNAPSHOT_INTERVAL    = 0.1f;
  const float INTERPOLATION_OFFSET = 0.2f;

  void Update() {
    // server logic
    ServerMovement();
    ServerSnapshot();

    // client logic
    ClientReceiveDataFromServer();
    ClientRenderLatestPostion();
  }

  void ClientReceiveDataFromServer() {
    while (_clientNetworkSimulationQueue.Count > 0 && _clientNetworkSimulationQueue.Peek().DeliveryTime < Time.time) {
      
      // this is our first snapshot
      if (_clientSnapshots.Count == 0) {
        _clientInterpolationTime = _clientNetworkSimulationQueue.Peek().Time - INTERPOLATION_OFFSET;
      }
      
      _clientSnapshots.Add(_clientNetworkSimulationQueue.Dequeue());
    }
  }

  void ClientRenderLatestPostion() {
    if (_clientSnapshots.Count > 0) {
      _clientInterpolationTime += Time.unscaledDeltaTime;

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
            var current = _clientSnapshots[t].Time - _clientInterpolationTime;

            interpAlpha = 1 - Mathf.Clamp01(current / range);

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