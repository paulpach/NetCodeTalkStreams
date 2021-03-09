/*

// PLAYER
- an abstraction
- to allow anyone/anything to act in the game as a player
- not tied to being client/server or similar
- several players can be assigned to a single client or server

// GAME CLIENT OR GAME SERVER - SIMULATION/WORLD
- server can be connected to
- client connects to someone

// CONNECTION
- server has connection to a client
- server can have many clients connected

- client has connection to a server
- client can only be connected to one server

// TRANSPORT
- connection abstraction
- handhsakes
- reliablity, notify
- ping calculations
- basically everything that has to do with sockets/transfering data

// SOCKET
- lowest level of networking
- only provides raw send/recv/poll calls
- only needs to send/recv unreliable



*/

using System;
using System.Numerics;

namespace BitPacking {
  class BitBuffer {
    // 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00011111

    const int   BITCOUNT   = 64;
    const int   USEDMASK   = BITCOUNT - 1;
    const int   INDEXSHIFT = 6;
    const ulong MAXVALUE   = ulong.MaxValue;

    ulong[] _data;
    int     _offsetInBits;

    public BitBuffer(int size) {
      Assert.Check(size > 0);

      _data         = new ulong[Native.RoundToAlignment(size, sizeof(ulong)) / sizeof(ulong)];
      _offsetInBits = 0;
    }

    // int = 32 bits
    // 11100000 00000000 00000011 11111111

    // ulong = 64 bits
    // 00000000 00000000 00000000 00000000 11100000 00000000 00000011 11111111


    public readonly struct FloatCompression {
      public readonly int   Min;
      public readonly int   Max;
      public readonly float Accuracy;

      public FloatCompression(int min, int max, float accuracy) {
        Min      = min;
        Max      = max;
        Accuracy = accuracy;
      }
    }

    public unsafe class FloatCompressor {
      int   _bits;
      uint  _range;
      float _min;
      float _max;
      float _accuracy;
      float _accuracyInv;

      public int Bits {
        get => _bits;
      }

      public FloatCompressor(FloatCompression compression) {
        if (compression.Min < compression.Max) {
          _min = compression.Min;
          _max = compression.Max;

          _accuracy    = compression.Accuracy;
          _accuracyInv = 1.0f / _accuracy;

          _range = (uint) ((compression.Max - compression.Min) * (1.0 / _accuracy));

          _bits = Maths.BitsRequiredForNumber(_range);
        } else {
          _min         = 0;
          _max         = 0;
          _range       = 0;
          _accuracy    = 0;
          _accuracyInv = 0;

          _bits = 32;
        }
      }

      public uint Compress(float value) {
        return Maths.Min((uint) (((value + -_min) * _accuracyInv) + 0.5f), _range);
      }

      public float Decompress(uint value) {
        return Maths.Clamp((value * _accuracy) + _min, _min, _max);
      }
    }

    // blocksize = 2-32
    // 32/8 = 4 chunks
    
    // some random uint value:
    // -> 00000000 00000000 00011100 00000110
    //                         13th bit - 12th index

    public void WriteUInt32VarLength(uint value, int blockSize) {
      var blocks = (Maths.BitScanReverse(value) + blockSize) / blockSize;

      // write data
      WriteInternal(1UL << (blocks - 1), blocks);
      WriteInternal(value, blocks * blockSize);
    }
    
    // 
    // 1UL << (2-1)
    // 10 <-

    public uint ReadUInt32VarLength(int blockSize) {
      var blocks = 1;

      while (ReadBoolean() == false) {
        ++blocks;
      }

      return ReadUInt32(blocks * blockSize);
    }

    public bool ReadBoolean() {
      return ReadInternal(1) == 1;
    }

    public uint ReadUInt32(int bits) {
      return (uint) ReadInternal(bits);
    }

    public unsafe void Write(float value) {
      WriteInternal(*(uint*) &value, 32);
    }

    public unsafe void Write(double value) {
      WriteInternal(*(ulong*) &value, 64);
    }

    public unsafe void WriteCompressedFloat(float value, int min, int max, int accuracy) {
      var q = (int) ((value * accuracy) + 0.5f);
      q += (-min * accuracy);
      q =  Maths.Clamp(q, 0, (max * accuracy) + (-min * accuracy));
      Write(q, Maths.BitsRequiredForNumber((max * accuracy) + (-min * accuracy)));
    }

    public void Write(uint value, int bits = 32) {
      Assert.Check(bits >= 0 && bits <= 32);
      WriteInternal(value, bits);
    }

    public void Write(int value, int bits = 32) {
      Assert.Check(bits >= 0 && bits <= 32);
      WriteInternal((ulong) value, bits);
    }

    ulong ReadInternal(int bits) {
      Assert.Check(bits >= 0 && bits <= 64);

      if (bits == 0) {
        return 0;
      }

      int   p             = _offsetInBits >> INDEXSHIFT;
      int   bitsUsed      = _offsetInBits & USEDMASK;
      ulong first         = _data[p] >> bitsUsed;
      int   remainingBits = bits - (BITCOUNT - bitsUsed);

      ulong value;

      if (remainingBits == 0) {
        value = (first & (MAXVALUE >> (BITCOUNT - bits)));
      } else {
        ulong second = _data[p + 1] & (MAXVALUE >> (BITCOUNT - remainingBits));
        value = (first | (second << (bits - remainingBits)));
      }

      _offsetInBits += bits;
      return value;
    }


    void WriteInternal(ulong value, int bits) {
      Assert.Check(bits >= 0 && bits <= 64);

      if (bits == 0) {
        return;
      }

      value &= (MAXVALUE >> (BITCOUNT - bits));

      // our current index
      var p = _offsetInBits >> INDEXSHIFT;

      // how many bits are currently _used_ in this index
      var bitsUsed = _offsetInBits & USEDMASK;
      var bitsFree = BITCOUNT - bitsUsed;
      var bitsLeft = bitsFree - bits;

      if (bitsLeft >= 0) {
        ulong mask = (MAXVALUE >> bitsFree) | (MAXVALUE << (BITCOUNT - bitsLeft));
        _data[p] = (_data[p] & mask) | (value << bitsUsed);
      } else {
        _data[p]     = ((_data[p] & (MAXVALUE >> bitsFree)) | (value << bitsUsed));
        _data[p + 1] = ((_data[p + 1] & (MAXVALUE << (bits - bitsFree))) | (value >> bitsFree));
      }

      _offsetInBits += bits;
    }
  }

  public class Transform {
    public Vector3    Position;
    public Quaternion Rotation;
  }

  unsafe class Program {
    static void Main(string[] args) {
      var transforms = new Transform[10];

      for (int i = 0; i < transforms.Length; ++i) {
        transforms[i] = new Transform();
      }

      var b = new BitBuffer(1200);

      foreach (var t in transforms) {
        b.WriteCompressedFloat(t.Position.X, -128, +128, 100);
        b.WriteCompressedFloat(t.Position.Z, -128, +128, 100);
        b.WriteCompressedFloat(t.Position.Y, 0, +64, 100);

        b.WriteCompressedFloat(t.Rotation.X, -1, +1, 100);
        b.WriteCompressedFloat(t.Rotation.Y, -1, +1, 100);
        b.WriteCompressedFloat(t.Rotation.Z, -1, +1, 100);
        b.WriteCompressedFloat(t.Rotation.W, -1, +1, 100);
      }


      // 11111111 11111111 11111111 11111111
      //int x = -1;
      //ulong u = (ulong) x;
      //const ulong MAX = ulong.MaxValue;
      // float  = 32 bits
      // double = 64 bits
      // 0111111 11110011 11111111 11110011
      //var x = -424.312f;
      //var p = (uint*)&x;
      //*p = *p & 0x7FFFFFFF;
      //Console.WriteLine(x);
      //Console.WriteLine((uint)x);
      //Console.WriteLine(*(uint*)&x);
      //
      // const int ACCURACY = 100;
      //
      // //
      // int min = -512;
      // int max = +512;
      //
      // var x = -424.312556f;
      // var q = (int)(x * ACCURACY);
      //
      // Console.WriteLine(x);
      // Console.WriteLine(q);
      //
      // Console.WriteLine(q + (-min * ACCURACY));
      //
      // // 
      // Console.WriteLine((max * ACCURACY) + (-min * ACCURACY));
      //
      // var offsetRangeXZ = (max * ACCURACY) + (-min * ACCURACY);
      // Console.WriteLine(Maths.BitsRequiredForNumber(offsetRangeXZ));
      //
      //
      // var offsetRangeY = (128 * ACCURACY) + (0 * ACCURACY);
      // Console.WriteLine(Maths.BitsRequiredForNumber(offsetRangeY));
    }
  }
}