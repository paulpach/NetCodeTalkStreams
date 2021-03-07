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
    int       size0, int       size1, int size2, int size3, int size4, int size5, int size6, int size7, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5,
    out void* ptr6,  out void* ptr7,  int alignment = ALIGNMENT
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


  public static int MallocAndClearBlock(
    int size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5,
    out void* ptr6, out void* ptr7, out void* ptr8, int alignment = ALIGNMENT
  ) {
    size0 = RoundToAlignment(size0, alignment);
    size1 = RoundToAlignment(size1, alignment);
    size2 = RoundToAlignment(size2, alignment);
    size3 = RoundToAlignment(size3, alignment);
    size4 = RoundToAlignment(size4, alignment);
    size5 = RoundToAlignment(size5, alignment);
    size6 = RoundToAlignment(size6, alignment);
    size7 = RoundToAlignment(size7, alignment);
    size8 = RoundToAlignment(size8, alignment);

    var size = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + 0;
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
    ptr  += size7;
    ptr8 =  ptr;


    Assert.Check(IsPointerAligned(ptr0, alignment));
    Assert.Check(IsPointerAligned(ptr1, alignment));
    Assert.Check(IsPointerAligned(ptr2, alignment));
    Assert.Check(IsPointerAligned(ptr3, alignment));
    Assert.Check(IsPointerAligned(ptr4, alignment));
    Assert.Check(IsPointerAligned(ptr5, alignment));
    Assert.Check(IsPointerAligned(ptr6, alignment));
    Assert.Check(IsPointerAligned(ptr7, alignment));
    Assert.Check(IsPointerAligned(ptr8, alignment));

    return size;
  }


  public static int MallocAndClearBlock(
    int       size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, int size9, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4,
    out void* ptr5,  out void* ptr6, out void* ptr7, out void* ptr8, out void* ptr9, int alignment = ALIGNMENT
  ) {
    size0 = RoundToAlignment(size0, alignment);
    size1 = RoundToAlignment(size1, alignment);
    size2 = RoundToAlignment(size2, alignment);
    size3 = RoundToAlignment(size3, alignment);
    size4 = RoundToAlignment(size4, alignment);
    size5 = RoundToAlignment(size5, alignment);
    size6 = RoundToAlignment(size6, alignment);
    size7 = RoundToAlignment(size7, alignment);
    size8 = RoundToAlignment(size8, alignment);
    size9 = RoundToAlignment(size9, alignment);

    var size = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + size9 + 0;
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
    ptr  += size7;
    ptr8 =  ptr;
    ptr  += size8;
    ptr9 =  ptr;


    Assert.Check(IsPointerAligned(ptr0, alignment));
    Assert.Check(IsPointerAligned(ptr1, alignment));
    Assert.Check(IsPointerAligned(ptr2, alignment));
    Assert.Check(IsPointerAligned(ptr3, alignment));
    Assert.Check(IsPointerAligned(ptr4, alignment));
    Assert.Check(IsPointerAligned(ptr5, alignment));
    Assert.Check(IsPointerAligned(ptr6, alignment));
    Assert.Check(IsPointerAligned(ptr7, alignment));
    Assert.Check(IsPointerAligned(ptr8, alignment));
    Assert.Check(IsPointerAligned(ptr9, alignment));

    return size;
  }


  public static int MallocAndClearBlock(
    int       size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, int size9, int size10, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3,
    out void* ptr4,  out void* ptr5, out void* ptr6, out void* ptr7, out void* ptr8, out void* ptr9, out void* ptr10, int alignment = ALIGNMENT
  ) {
    size0  = RoundToAlignment(size0, alignment);
    size1  = RoundToAlignment(size1, alignment);
    size2  = RoundToAlignment(size2, alignment);
    size3  = RoundToAlignment(size3, alignment);
    size4  = RoundToAlignment(size4, alignment);
    size5  = RoundToAlignment(size5, alignment);
    size6  = RoundToAlignment(size6, alignment);
    size7  = RoundToAlignment(size7, alignment);
    size8  = RoundToAlignment(size8, alignment);
    size9  = RoundToAlignment(size9, alignment);
    size10 = RoundToAlignment(size10, alignment);

    var size = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + size9 + size10 + 0;
    var ptr  = (byte*) MallocAndClear(size);
    ptr0  =  ptr;
    ptr   += size0;
    ptr1  =  ptr;
    ptr   += size1;
    ptr2  =  ptr;
    ptr   += size2;
    ptr3  =  ptr;
    ptr   += size3;
    ptr4  =  ptr;
    ptr   += size4;
    ptr5  =  ptr;
    ptr   += size5;
    ptr6  =  ptr;
    ptr   += size6;
    ptr7  =  ptr;
    ptr   += size7;
    ptr8  =  ptr;
    ptr   += size8;
    ptr9  =  ptr;
    ptr   += size9;
    ptr10 =  ptr;


    Assert.Check(IsPointerAligned(ptr0, alignment));
    Assert.Check(IsPointerAligned(ptr1, alignment));
    Assert.Check(IsPointerAligned(ptr2, alignment));
    Assert.Check(IsPointerAligned(ptr3, alignment));
    Assert.Check(IsPointerAligned(ptr4, alignment));
    Assert.Check(IsPointerAligned(ptr5, alignment));
    Assert.Check(IsPointerAligned(ptr6, alignment));
    Assert.Check(IsPointerAligned(ptr7, alignment));
    Assert.Check(IsPointerAligned(ptr8, alignment));
    Assert.Check(IsPointerAligned(ptr9, alignment));
    Assert.Check(IsPointerAligned(ptr10, alignment));

    return size;
  }


  public static int MallocAndClearBlock(
    int       size0, int       size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, int size9, int size10, int size11, out void* ptr0, out void* ptr1, out void* ptr2,
    out void* ptr3,  out void* ptr4,  out void* ptr5, out void* ptr6, out void* ptr7, out void* ptr8, out void* ptr9, out void* ptr10, out void* ptr11, int alignment = ALIGNMENT
  ) {
    size0  = RoundToAlignment(size0, alignment);
    size1  = RoundToAlignment(size1, alignment);
    size2  = RoundToAlignment(size2, alignment);
    size3  = RoundToAlignment(size3, alignment);
    size4  = RoundToAlignment(size4, alignment);
    size5  = RoundToAlignment(size5, alignment);
    size6  = RoundToAlignment(size6, alignment);
    size7  = RoundToAlignment(size7, alignment);
    size8  = RoundToAlignment(size8, alignment);
    size9  = RoundToAlignment(size9, alignment);
    size10 = RoundToAlignment(size10, alignment);
    size11 = RoundToAlignment(size11, alignment);

    var size = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + size9 + size10 + size11 + 0;
    var ptr  = (byte*) MallocAndClear(size);
    ptr0  =  ptr;
    ptr   += size0;
    ptr1  =  ptr;
    ptr   += size1;
    ptr2  =  ptr;
    ptr   += size2;
    ptr3  =  ptr;
    ptr   += size3;
    ptr4  =  ptr;
    ptr   += size4;
    ptr5  =  ptr;
    ptr   += size5;
    ptr6  =  ptr;
    ptr   += size6;
    ptr7  =  ptr;
    ptr   += size7;
    ptr8  =  ptr;
    ptr   += size8;
    ptr9  =  ptr;
    ptr   += size9;
    ptr10 =  ptr;
    ptr   += size10;
    ptr11 =  ptr;


    Assert.Check(IsPointerAligned(ptr0, alignment));
    Assert.Check(IsPointerAligned(ptr1, alignment));
    Assert.Check(IsPointerAligned(ptr2, alignment));
    Assert.Check(IsPointerAligned(ptr3, alignment));
    Assert.Check(IsPointerAligned(ptr4, alignment));
    Assert.Check(IsPointerAligned(ptr5, alignment));
    Assert.Check(IsPointerAligned(ptr6, alignment));
    Assert.Check(IsPointerAligned(ptr7, alignment));
    Assert.Check(IsPointerAligned(ptr8, alignment));
    Assert.Check(IsPointerAligned(ptr9, alignment));
    Assert.Check(IsPointerAligned(ptr10, alignment));
    Assert.Check(IsPointerAligned(ptr11, alignment));

    return size;
  }
}