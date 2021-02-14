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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Transport {
  public static unsafe class Maths {
    static byte[] _debruijnTable = {
      0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30,
      8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct FastAbs {
      public const uint Mask = 0x7FFFFFFF;

      [FieldOffset(0)]
      public uint UInt32;

      [FieldOffset(0)]
      public float Single;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfBits<T>() where T : unmanaged {
      return sizeof(T) * 8;
    }

    public static int BytesRequiredForBits(int b) {
      return (b + 7) >> 3;
    }

    public static int BitsRequiredForNumber(int n) {
      for (int i = 31; i >= 0; --i) {
        int b = 1 << i;

        if ((n & b) == b) {
          return i + 1;
        }
      }

      return 0;
    }

    public static int FloorToInt(double value) {
      return (int) Math.Floor(value);
    }

    public static int CeilToInt(double value) {
      return (int) Math.Ceiling(value);
    }

    public static int CountUsedBitsMinOne(uint value) {
      Assert.Check(value > 0);

      var bits = 0;

      do {
        bits++;
        value >>= 1;
      } while (value > 0);

      return bits;
    }

    public static int BitsRequiredForNumber(uint n) {
      for (int i = 31; i >= 0; --i) {
        int b = 1 << i;

        if ((n & b) == b) {
          return i + 1;
        }
      }

      return 0;
    }

    public static uint NextPowerOfTwo(uint v) {
      v--;
      v |= v >> 1;
      v |= v >> 2;
      v |= v >> 4;
      v |= v >> 8;
      v |= v >> 16;
      v++;
      return v;
    }

    public static double MillisecondsToSeconds(double seconds) {
      return (seconds / 1000.0);
    }

    public static long SecondsToMilliseconds(double seconds) {
      return (long) (seconds * 1000.0);
    }

    public static long SecondsToMicroseconds(double seconds) {
      return (long) (seconds * 1000000.0);
    }

    public static double MicrosecondsToSeconds(long microseconds) {
      return microseconds / 1000000.0;
    }

    public static long MillisecondsToMicroseconds(long milliseconds) {
      return milliseconds * 1000;
    }

    public static byte ClampToByte(int v) {
      if (v < 0) {
        return 0;
      }

      if (v > 255) {
        return 255;
      }

      return (byte) v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ZigZagEncode(int i) {
      return (i >> 31) ^ (i << 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ZigZagDecode(int i) {
      return (i >> 1) ^ -(i & 1);
    }

    public static int Clamp(int v, int min, int max) {
      if (v < min) {
        return min;
      }

      if (v > max) {
        return max;
      }

      return v;
    }

    public static uint Clamp(uint v, uint min, uint max) {
      if (v < min) {
        return min;
      }

      if (v > max) {
        return max;
      }

      return v;
    }


    public static double Clamp(double v, double min, double max) {
      if (v < min) {
        return min;
      }

      if (v > max) {
        return max;
      }

      return v;
    }


    public static float Clamp(float v, float min, float max) {
      if (v < min) {
        return min;
      }

      if (v > max) {
        return max;
      }

      return v;
    }

    public static double Clamp01(double v) {
      if (v < 0) {
        return 0;
      }

      if (v > 1) {
        return 1;
      }

      return v;
    }

    public static float Clamp01(float v) {
      if (v < 0f) {
        return 0f;
      }

      if (v > 1f) {
        return 1f;
      }

      return v;
    }

    public static float Lerp(float a, float b, float t) {
      return a + ((b - a) * Clamp01(t));
    }

    public static uint Min(uint v, uint max) {
      if (v > max) {
        return max;
      }

      return v;
    }

    public static int BitScanReverse(int v) {
      return BitScanReverse(unchecked((uint) v));
    }

    public static int BitScanReverse(uint v) {
      v |= v >> 1;
      v |= v >> 2;
      v |= v >> 4;
      v |= v >> 8;
      v |= v >> 16;
      return _debruijnTable[(v * 0x07C4ACDDU) >> 27];
    }
  }
}