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

namespace Transport {
  unsafe partial class Native {
    public static int MallocAndClearBlock(
      int size0, int size1, out void* ptr0, out void* ptr1, int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);

      var size = size0 + size1 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));

      return size;
    }


    public static int MallocAndClearBlock(
      int size0, int size1, int size2, out void* ptr0, out void* ptr1, out void* ptr2, int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);
      size2 = RoundToAlignment(size2, alignment);

      var size = size0 + size1 + size2 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;
      ptr  += size1;
      ptr2 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));
      Assert.Check(IsPointerAligned(ptr2, alignment));

      return size;
    }


    public static int MallocAndClearBlock(
      int size0, int size1, int size2, int size3, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);
      size2 = RoundToAlignment(size2, alignment);
      size3 = RoundToAlignment(size3, alignment);

      var size = size0 + size1 + size2 + size3 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;
      ptr  += size1;
      ptr2 =  ptr;
      ptr  += size2;
      ptr3 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));
      Assert.Check(IsPointerAligned(ptr2, alignment));
      Assert.Check(IsPointerAligned(ptr3, alignment));

      return size;
    }


    public static int MallocAndClearBlock(
      int size0, int size1, int size2, int size3, int size4, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);
      size2 = RoundToAlignment(size2, alignment);
      size3 = RoundToAlignment(size3, alignment);
      size4 = RoundToAlignment(size4, alignment);

      var size = size0 + size1 + size2 + size3 + size4 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;
      ptr  += size1;
      ptr2 =  ptr;
      ptr  += size2;
      ptr3 =  ptr;
      ptr  += size3;
      ptr4 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));
      Assert.Check(IsPointerAligned(ptr2, alignment));
      Assert.Check(IsPointerAligned(ptr3, alignment));
      Assert.Check(IsPointerAligned(ptr4, alignment));

      return size;
    }


    public static int MallocAndClearBlock(
      int size0, int size1, int size2, int size3, int size4, int size5, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);
      size2 = RoundToAlignment(size2, alignment);
      size3 = RoundToAlignment(size3, alignment);
      size4 = RoundToAlignment(size4, alignment);
      size5 = RoundToAlignment(size5, alignment);

      var size = size0 + size1 + size2 + size3 + size4 + size5 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;
      ptr  += size1;
      ptr2 =  ptr;
      ptr  += size2;
      ptr3 =  ptr;
      ptr  += size3;
      ptr4 =  ptr;
      ptr  += size4;
      ptr5 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));
      Assert.Check(IsPointerAligned(ptr2, alignment));
      Assert.Check(IsPointerAligned(ptr3, alignment));
      Assert.Check(IsPointerAligned(ptr4, alignment));
      Assert.Check(IsPointerAligned(ptr5, alignment));

      return size;
    }


    public static int MallocAndClearBlock(
      int size0, int size1, int size2, int size3, int size4, int size5, int size6, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6,
      int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);
      size2 = RoundToAlignment(size2, alignment);
      size3 = RoundToAlignment(size3, alignment);
      size4 = RoundToAlignment(size4, alignment);
      size5 = RoundToAlignment(size5, alignment);
      size6 = RoundToAlignment(size6, alignment);

      var size = size0 + size1 + size2 + size3 + size4 + size5 + size6 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;
      ptr  += size1;
      ptr2 =  ptr;
      ptr  += size2;
      ptr3 =  ptr;
      ptr  += size3;
      ptr4 =  ptr;
      ptr  += size4;
      ptr5 =  ptr;
      ptr  += size5;
      ptr6 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));
      Assert.Check(IsPointerAligned(ptr2, alignment));
      Assert.Check(IsPointerAligned(ptr3, alignment));
      Assert.Check(IsPointerAligned(ptr4, alignment));
      Assert.Check(IsPointerAligned(ptr5, alignment));
      Assert.Check(IsPointerAligned(ptr6, alignment));

      return size;
    }


    public static int MallocAndClearBlock(
      int       size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5,
      out void* ptr6,  out void* ptr7, int alignment = ALIGNMENT
    ) {
      size0 = RoundToAlignment(size0, alignment);
      size1 = RoundToAlignment(size1, alignment);
      size2 = RoundToAlignment(size2, alignment);
      size3 = RoundToAlignment(size3, alignment);
      size4 = RoundToAlignment(size4, alignment);
      size5 = RoundToAlignment(size5, alignment);
      size6 = RoundToAlignment(size6, alignment);
      size7 = RoundToAlignment(size7, alignment);

      var size = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + 0;
      var ptr  = (byte*) MallocAndClear(size);
      ptr0 =  ptr;
      ptr  += size0;
      ptr1 =  ptr;
      ptr  += size1;
      ptr2 =  ptr;
      ptr  += size2;
      ptr3 =  ptr;
      ptr  += size3;
      ptr4 =  ptr;
      ptr  += size4;
      ptr5 =  ptr;
      ptr  += size5;
      ptr6 =  ptr;
      ptr  += size6;
      ptr7 =  ptr;


      Assert.Check(IsPointerAligned(ptr0, alignment));
      Assert.Check(IsPointerAligned(ptr1, alignment));
      Assert.Check(IsPointerAligned(ptr2, alignment));
      Assert.Check(IsPointerAligned(ptr3, alignment));
      Assert.Check(IsPointerAligned(ptr4, alignment));
      Assert.Check(IsPointerAligned(ptr5, alignment));
      Assert.Check(IsPointerAligned(ptr6, alignment));
      Assert.Check(IsPointerAligned(ptr7, alignment));

      return size;
    }
  }
}