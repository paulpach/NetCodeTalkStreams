using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeGeneration.Generator {
  class Program {
    enum MyPrefabs {
      // ... 

      Player,
      Enemy,
      House,
    }

    static string[] GetAllPrefabNamesInProject() =>
      new string[] {
        "Player", "Enemy", "House"
      };

    abstract class CG_Type {
      public string Name;
    }

    class CG_Class : CG_Type {
      public List<(Type, string)> Fields = new List<(Type, string)>();
    }

    class CG_Struct : CG_Type {
      public List<(Type, string)> Fields = new List<(Type, string)>();
    }

    class CG_Component : CG_Type {
      public bool IsUnique;
      public bool UseStrictMemoryLayout;
      public bool AllowAutoSerializationOnNetwork;
    }
    
    /*
     
     *.qtn
     
     enum Class {
       Rogue, Warrior, Mage
     }
     
     
     component Character {
       int Health;
       int Level;
       int Mana;
       Class Class;
     } 
     
     signals, events, input-blocks. 
     
     */
    static void Main(string[] args) {
      // Assembly-CSharp.dll, main unity dll
      // Roslyn Source Generators ... unity support is "iffy"

      const string OUTPUT_PATH = "../../../../CodeGeneration.Target/Generated.cs";


      var input = new List<CG_Type>();

      var foo = new CG_Class() {Name  = "Foo"};
      foo.Fields.Add((typeof(int), "test"));
      
      var bar = new CG_Struct() {Name = "Bar"};
      bar.Fields.Add((typeof(int), "test"));
      
      input.Add(foo);
      input.Add(bar);

      var builder = new CodeBuilder();
      builder.Using("System.Runtime.InteropServices");

      using (builder.BeginNamespace("TestNamespace")) {

        foreach (var type in input) {

          if (type is CG_Class cls) {
            GenerateClass(builder, cls);
          }

          if (type is CG_Struct str) {
            GenerateStruct(builder, str);
          }
        }
      }

      File.WriteAllText(OUTPUT_PATH, builder.ToString());
    }

    static void GenerateStruct(CodeBuilder builder, CG_Struct str) {
      using (builder.BeginStruct(str.Name, LayoutKind.Sequential)) {
        GenerateFields(builder, str.Fields);
      }
    }

    static void GenerateClass(CodeBuilder builder, CG_Class cls) {
      using (builder.BeginClass(cls.Name)) {
        GenerateFields(builder, cls.Fields);
      }
    }

    static void GenerateFields(CodeBuilder builder, List<(Type, string)> clsFields) {
      foreach (var (type, name) in clsFields) {
        builder.Field(type.FullName, name);
      }
    }
  }
}
