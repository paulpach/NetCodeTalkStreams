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
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Transport {
  public static unsafe partial class Native {
    public const int ALIGNMENT       = 8;
    public const int CACHE_LINE_SIZE = 64;

    public static void* Malloc(int size) {
      return UnsafeUtility.Malloc(size, ALIGNMENT, Allocator.Persistent);
    }

    public static void Free(void* memory) {
      UnsafeUtility.Free(memory, Allocator.Persistent);
    }

    public static void MemClear(void* ptr, int size) {
      UnsafeUtility.MemSet(ptr, 0, size); 
    }

    public static void MemCpy(void* d, void* s, int size) {
      UnsafeUtility.MemCpy(d, s, size);
    }

    public static void* MallocAndClear(int size) {
      var memory = Malloc(size);
      MemClear(memory, size);
      return memory;
    }

    public static T* MallocAndClear<T>() where T : unmanaged {
      var memory = Malloc<T>();
      MemClear(memory, sizeof(T));
      return memory;
    }

    public static T* Malloc<T>() where T : unmanaged {
      return (T*) Malloc(sizeof(T));
    }

    public static void* MallocAndClearArray(int stride, int length) {
      var ptr = Malloc(stride * length);
      MemClear(ptr, stride * length);
      return ptr;
    }

    public static T* MallocAndClearArray<T>(int length) where T : unmanaged {
      return (T*) MallocAndClearArray(sizeof(T), length);
    }

    public static T** MallocAndClearPtrArray<T>(int length) where T : unmanaged {
      return (T**) MallocAndClearArray(sizeof(T*), length);
    }

    public static void ArrayCopy(void* source, int sourceIndex, void* destination, int destinationIndex, int count, int elementStride) {
      MemCpy(((byte*) destination) + (destinationIndex * elementStride), ((byte*) source) + (sourceIndex * elementStride), count * elementStride);
    }

    public static void ArrayClear<T>(T* ptr, int size) where T : unmanaged {
      MemClear(ptr, sizeof(T) * size);
    }

    public static T* DoubleArray<T>(T* array, int currentLength) where T : unmanaged {
      return ExpandArray(array, currentLength, currentLength * 2);
    }

    public static T* ExpandArray<T>(T* array, int currentLength, int newLength) where T : unmanaged {
      Assert.Check(newLength > currentLength);

      var oldArray = array;
      var newArray = MallocAndClearArray<T>(newLength);

      // copy old contents
      MemCpy(newArray, oldArray, sizeof(T) * currentLength);

      // free old buffer
      Free(oldArray);

      // return the new size
      return newArray;
    }

    public static T** DoublePtrArray<T>(T** array, int currentLength) where T : unmanaged {
      return ExpandPtrArray(array, currentLength, currentLength * 2);
    }

    public static T** ExpandPtrArray<T>(T** array, int currentLength, int newLength) where T : unmanaged {
      Assert.Check(newLength > currentLength);

      var oldArray = array;
      var newArray = MallocAndClearPtrArray<T>(newLength);

      // copy old contents
      MemCpy(newArray, oldArray, sizeof(T*) * currentLength);

      // free old buffer
      Free(oldArray);

      // return the new size
      return newArray;
    }

    public static void* Expand(void* buffer, int currentSize, int newSize) {
      Assert.Check(newSize > currentSize);

      var oldBuffer = buffer;
      var newBuffer = MallocAndClear(newSize);

      // copy old contents
      MemCpy(newBuffer, oldBuffer, currentSize);

      // free old buffer
      Free(oldBuffer);

      // return the new size
      return newBuffer;
    }

    public static void MemCpyFast(void* d, void* s, int size) {
      switch (size) {
        case 4:
          *(uint*) d = *(uint*) s;
          break;

        case 8:
          *(ulong*) d = *(ulong*) s;
          break;

        case 12:
          *((ulong*) d)      = *((ulong*) s);
          *(((uint*) d) + 2) = *(((uint*) s) + 2);
          break;

        case 16:
          *((ulong*) d)     = *((ulong*) s);
          *((ulong*) d + 1) = *((ulong*) s + 1);
          break;

        default:
          MemCpy(d, s, size);
          break;
      }
    }

    public static bool IsPointerAligned(void* pointer, int alignment) {
      return ((long) pointer % alignment) == 0;
    }

    public static void* AlignPointer(void* pointer, int alignment) {
      var address = (long) pointer;

      // only adjust if not already aligned
      if ((address % alignment) != 0) {
        return ((byte*) pointer) + (alignment - (address % alignment));
      }

      return pointer;
    }

    public static int RoundToMaxAlignment(int stride) {
      return RoundToAlignment(stride, ALIGNMENT);
    }

    public static int RoundToAlignment(int stride, int alignment) {
      switch (alignment) {
        case 1: return ((stride));
        case 2: return ((stride + 1) >> 1) * 2;
        case 4: return ((stride + 3) >> 2) * 4;
        case 8: return ((stride + 7) >> 3) * 8;
        case 16: return ((stride + 15) >> 4) * 16;
        default:
          throw new InvalidOperationException($"Invalid Alignment: {alignment}");
      }
    }

    public static T Empty<T>() where T : unmanaged {
      return default;
    }

    public static int RoundBitsTo64(int bits) {
      return ((bits + 63) >> 6) * 64;
    }

    public static int GetAlignment<T>() where T : unmanaged {
      return GetAlignment(sizeof(T));
    }

    public static int GetAlignment(int stride) {
      if ((stride % 8) == 0) {
        return 8;
      }

      if ((stride % 4) == 0) {
        return 4;
      }

      return (stride % 2) == 0 ? 2 : 1;
    }

    public static int GetMaxAlignment(int a, int b) {
      return Math.Max(GetAlignment(a), GetAlignment(b));
    }

    public static int GetMaxAlignment(int a, int b, int c) {
      return Math.Max(GetMaxAlignment(a, b), GetAlignment(c));
    }

    public static int GetMaxAlignment(int a, int b, int c, int d) {
      return Math.Max(GetMaxAlignment(a, b, c), GetAlignment(d));
    }

    public static int GetMaxAlignment(int a, int b, int c, int d, int e) {
      return Math.Max(GetMaxAlignment(a, b, c, e), GetAlignment(e));
    }
  }
}