using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

public static unsafe partial class Native {
  public const int ALIGNMENT       = 8;
  public const int CACHE_LINE_SIZE = 64;

  [SuppressUnmanagedCodeSecurity]
  [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
  static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
  static extern IntPtr memmove(IntPtr dest, IntPtr src, UIntPtr count);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
  static extern IntPtr memset(IntPtr dest, int c, UIntPtr byteCount);

  public static void* Malloc(int size) {
    return Marshal.AllocHGlobal(size).ToPointer();
  }

  public static void Free(void* ptr) {
    Assert.Check(ptr != null);
    Marshal.FreeHGlobal(new IntPtr(ptr));
  }

  public static void MemClear(void* ptr, int size) {
    Assert.Check(ptr != null);
    memset((IntPtr) ptr, 0, (UIntPtr) size);
  }

  public static void MemMove(void* d, void* s, int size) {
    Assert.Check(d != null);
    Assert.Check(s != null);
    memmove((IntPtr) d, (IntPtr) s, (UIntPtr) size);
  }

  public static void MemCpy(void* d, void* s, int size) {
    Assert.Check(d != null);
    Assert.Check(s != null);
    Assert.Check(size > 0);
    memcpy((IntPtr) d, (IntPtr) s, (UIntPtr) size);
  }

  public static void* MallocAndClear(int size) {
    var memory = Malloc(size);
    MemClear(memory, size);
    return memory;
  }

  public static T[] ToManagedArray<T>(T* buffer, int length) where T : unmanaged {
    var arr = new T[length];

    for (int i = 0; i < length; ++i) {
      arr[i] = buffer[i];
    }

    return arr;
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

  public static void ArrayPtrClear<T>(T** ptr, int size) where T : unmanaged {
    MemClear(ptr, sizeof(T*) * size);
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

  public static int WordCount(int stride, int wordSize) {
    return RoundToAlignment(stride, wordSize) / wordSize;
  }

  public static int RoundToAlignment(int stride, int alignment) {
    switch (alignment) {
      case 1: return ((stride));
      case 2: return ((stride + 1) >> 1) * 2;
      case 4: return ((stride + 3) >> 2) * 4;
      case 8: return ((stride + 7) >> 3) * 8;
      case 16: return ((stride + 15) >> 4) * 16;
      case 32: return ((stride + 31) >> 5) * 32;
      default:
        throw new InvalidOperationException($"Invalid Alignment: {alignment}");
    }
  }

  public static T Empty<T>() where T : unmanaged {
    return new T();
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

  public static string PrintBits(void* ptr, int bytes) {
    var sb = new StringBuilder();

    for (int i = 0; i < bytes; ++i) {
      var b = ((byte*) ptr)[i];

      for (Int32 n = 0; n < 8; ++n) {
        sb.Append((b & (1 << n)) == 0 ? "0" : "1");
      }

      sb.Append(" ");
    }

    return sb.ToString();
  }
}