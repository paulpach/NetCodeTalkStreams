using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/*
 
 // Network.NetworkTime = lastservertimeseen + smoothavg(ping/2)

 "classic networking" i.e. "fps networking" i.e. "state replication"
 
  // - interpolation time
  remote players - i.e. data you got from the server about other objects positions, etc. in the world
 
  // - prediction time, this is what i use to derive tick numbers 
  local  player - your own player that's using client side prediction
 
  // client is producing inputs and doing predictions with them
  
  client:
  50, 51, 52, 53 ... N 
  |   |   |   |      |
                     P
  
  server (input from client):
  50, 51, 52, 53 ... N
  |   |   |   |
  A   A   A   A
  
  eventually the client will receive A state for input 50:
    - you can look at the state for A and compare it to P, if they are identical u dont need to do anything 
  
  //
  
  - Always snap to server state when you receive it on the client
  - Don't try to  compare to client state because it limits the prediction and gives false "performance" target data
  - Don't even store clients state it doesnt matter
  - If you want to do "smooth" reconcilliation do it visually only
 */

public static class Maths {
  public const float EPSILON   = float.Epsilon;
  public const float RAD_2_DEG = 57.29578f;
  public const float DEG_2_RAD = 0.01745329f;

  static readonly byte[] _debruijnTable32 = {
    0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30,
    8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31
  };

  static readonly int[] _debruijnTable64 = new int[128] {
    0, // change to 1 if you want bitSize(0) = 1
    48, -1, -1, 31, -1, 15, 51, -1, 63, 5, -1, -1, -1, 19, -1,
    23, 28, -1, -1, -1, 40, 36, 46, -1, 13, -1, -1, -1, 34, -1, 58,
    -1, 60, 2, 43, 55, -1, -1, -1, 50, 62, 4, -1, 18, 27, -1, 39,
    45, -1, -1, 33, 57, -1, 1, 54, -1, 49, -1, 17, -1, -1, 32, -1,
    53, -1, 16, -1, -1, 52, -1, -1, -1, 64, 6, 7, 8, -1, 9, -1,
    -1, -1, 20, 10, -1, -1, 24, -1, 29, -1, -1, 21, -1, 11, -1, -1,
    41, -1, 25, 37, -1, 47, -1, 30, 14, -1, -1, -1, -1, 22, -1, -1,
    35, 12, -1, -1, -1, 59, 42, -1, -1, 61, 3, 26, 38, 44, -1, 56
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
  public static unsafe int SizeOfBits<T>() where T : unmanaged {
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
  public static long ZigZagEncode(long i) {
    return (i >> 63) ^ (i << 1);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static long ZigZagDecode(long i) {
    return (i >> 1) ^ -(i & 1);
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int BitScanReverse(int v) {
    return BitScanReverse(unchecked((uint) v));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int BitScanReverse(uint v) {
    v |= v >> 1;
    v |= v >> 2;
    v |= v >> 4;
    v |= v >> 8;
    v |= v >> 16;
    return _debruijnTable32[(v * 0x07C4ACDDU) >> 27];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int BitScanReverse(ulong v) {
    v |= v >> 1; // Round down to one less than a power of 2 
    v |= v >> 2;
    v |= v >> 4;
    v |= v >> 8;
    v |= v >> 16;
    v |= v >> 32;
    return _debruijnTable64[(v * 0x6c04f118e9966f6bUL) >> 57];
  }
}