using System.Runtime.InteropServices;
namespace TestNamespace {
  public unsafe partial class Foo  {
    public System.Int32 test;
  }
  [StructLayout(LayoutKind.Explicit)]
  public unsafe partial struct Bar {
    public System.Int32 test;
  }
}
