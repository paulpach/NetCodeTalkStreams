using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace CodeGeneration.Generator {
  public static class CodeDom {
    public static void Example() {
      CodeCompileUnit ccu = new CodeCompileUnit();
      CodeNamespace   cns = new CodeNamespace("Test");
      ccu.Namespaces.Add(cns);

      CodeDomProvider      provider = CodeDomProvider.CreateProvider("CSharp");
      CodeGeneratorOptions options  = new CodeGeneratorOptions();
      options.BracingStyle = "C";
      
      var result = new StringWriter();
      provider.GenerateCodeFromCompileUnit(ccu, result, options);
      Console.WriteLine(result.ToString());
    }
  }
}