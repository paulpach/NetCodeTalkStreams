using System;

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

    void Write(ulong value, int bits) {
      Assert.Check(bits >= 0 && bits <= 64);

      if (bits == 0) {
        return;
      }

      // MAXVALUE:
      // 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111

      // BITCOUNT: 64

      // example, bits: 10

      // BITCOUNT-bits = 54

      // RESULT:
      // 00000000 00000000 00000000 00000000 00000000 00000000 00000011 11111111

      value &= (MAXVALUE >> (BITCOUNT - bits));


      // our current index
      var p = _offsetInBits >> INDEXSHIFT;

      // how many bits are currently _used_ in this index
      var bitsUsed = _offsetInBits & USEDMASK;
      var bitsFree = BITCOUNT - bitsUsed;
      var bitsLeft = bitsFree - bits;

      //                                         X
      // 00000000 00000000 00000000 00011111 11111000 00000000 00000000 00000000

      //   X                                     
      // 11100000 00000000 00000000 00000000 00000000 00000000 00000000 00000000
      // 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00011111
      
      // all data fits into the current "p" index
      if (bitsLeft >= 0) {

        ulong mask = (MAXVALUE >> bitsFree) | (MAXVALUE << (BITCOUNT - bitsLeft));
        // 11111111 11111111 11111111 11100000 00000111 11111111 11111111 11111111
        
        // (value << bitsUsed)
        // before: 00000000 00000000 00000000 00000000 00000000 00000000 00000011 11111111
        // after:  00000000 00000000 00000000 00011111 11111000 00000000 00000000 00000000
        
        _data[p] = (_data[p] & mask) | (value << bitsUsed);

      } else {
        _data[p]     = ((_data[p] & (MAXVALUE >> bitsFree)) | (value << bitsUsed));
        _data[p + 1] = ((_data[p + 1] & (MAXVALUE << (bits - bitsFree))) | (value >> bitsFree));
      }

      _offsetInBits += bits;
    }

    // 32 bit integer
    // 18 bits

    // 18/8 = 2.25 = 2 bytes 2 bits = 3 bytes
    // 
  }

  class Program {
    static void Main(string[] args) {
      // 2.5x
    }
  }
}