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

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Transport {
  public struct Timer {
    long _start;
    long _elapsed;
    byte _running;

    public static Timer StartNew() {
      Timer t;
      t = default;
      t.Start();
      return t;
    }

    public long ElapsedInTicks {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _running == 1 ? _elapsed + GetDelta() : _elapsed;
    }

    public double ElapsedInMilliseconds {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ElapsedInSeconds * 1000.0;
    }

    public double ElapsedInSeconds {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ElapsedInTicks / (double) Stopwatch.Frequency;
    }

    public double Now {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ElapsedInSeconds;
    }

    public bool IsRunning {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _running == 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start() {
      if (_running == 0) {
        _start   = Stopwatch.GetTimestamp();
        _running = 1;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Stop() {
      var dt = GetDelta();

      if (_running == 1) {
        _elapsed += dt;
        _running =  0;

        if (_elapsed < 0) {
          _elapsed = 0;
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() {
      _elapsed = 0;
      _running = 0;
      _start   = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Restart() {
      _elapsed = 0;
      _running = 1;
      _start   = Stopwatch.GetTimestamp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    long GetDelta() {
      return Stopwatch.GetTimestamp() - _start;
    }
  }
}