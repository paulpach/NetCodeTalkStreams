using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeGeneration.Generator {
  public class CodeBuilder : IDisposable {
    const int INDENT = 2;

    public struct StringExpr {
      public string Value;

      public StringExpr(string expr) {
        Value = expr;
      }
    }

    int           _scope = 0;
    StringBuilder _sb    = new StringBuilder();

    public string Null => "null";

    public CodeBuilder BeginScope(string line) {
      Line(line + " {");
      ++_scope;
      return this;
    }

    public CodeBuilder BeginScope() {
      Line("{");
      ++_scope;
      return this;
    }

    public CodeBuilder BeginNamespace(string ns) {
      return BeginScope($"namespace {ns}");
    }

    public CodeBuilder BeginMethod(string name, string returnType = "void", string modifiers = "public", (string, string)[] args = null) {
      var argsString = string.Join(", ", (args ?? new (string, string)[0]).Select(x => x.Item1 + " " + x.Item2).ToArray());
      return BeginScope($"{AddSpace(modifiers)}{AddSpace(returnType)}{name}({argsString})");
    }

    public CodeBuilder BeginClass(string name, string modifiers = "public unsafe partial", string[] baseTypes = null) {
      var baseTypesString = string.Join(", ", baseTypes ?? new string[0]);
      if (baseTypesString.Length > 0) {
        baseTypesString = $": {baseTypesString} ";
      }

      return BeginScope($"{AddSpace(modifiers)}class {name} {baseTypesString}");
    }

    public CodeBuilder BeginStruct(string name, LayoutKind layout, string modifiers = "public unsafe partial", string[] interfaces = null) {
      var ifacesString = string.Join(", ", interfaces ?? new string[0]);
      if (ifacesString.Length > 0) {
        ifacesString = $" : {ifacesString}";
      }

      switch (layout) {
        case LayoutKind.Explicit:
          Attr("StructLayout", LayoutKind.Explicit);
          break;

        case LayoutKind.Sequential:
          Attr("StructLayout", LayoutKind.Explicit);
          break;
      }

      return BeginScope($"{AddSpace(modifiers)}struct {name}{ifacesString}");
    }

    public CodeBuilder BeginEnum(string name, string baseType = null, string modifiers = "public") {
      if (baseType == null) {
        baseType = "int";
      }

      return BeginScope($"{(AddSpace(modifiers))}enum {name} : {baseType}");
    }

    public CodeBuilder BeginProperty(string type, string name, string modifiers = "public") {
      return BeginScope($"{AddSpace(modifiers)}{type} {name}");
    }

    public CodeBuilder BeginGetter() {
      return BeginScope($"get");
    }

    public CodeBuilder BeginSetter() {
      return BeginScope($"set");
    }

    public CodeBuilder BeginIf(string expression) {
      return BeginScope($"if ({expression})");
    }

    public CodeBuilder BeginElseIf(string expression) {
      return BeginScope($"else if ({expression})");
    }

    public CodeBuilder BeginElse() {
      return BeginScope("else");
    }

    public CodeBuilder BeginFixed(string type, string var, string source) {
      return BeginScope($"fixed ({type}* {var} = {source})");
    }

    public CodeBuilder BeginWhile(string expression) {
      return BeginScope($"while ({expression})");
    }

    public CodeBuilder BeginFor(string init, string check, string incr) {
      return BeginScope($"for ({init}, {check}, {incr})");
    }

    public CodeBuilder BeginFor(string var, object compare) {
      return BeginScope($"for (int {var} = 0; {var} < {compare}; ++{var})");
    }

    public CodeBuilder BeginSwitch(string expression) {
      return BeginScope($"switch ({expression})");
    }

    public CodeBuilder BeginCase(object value) {
      return BeginScope($"case {value}:");
    }

    public void Label(string name) {
      Stmt(name + ": ");
    }

    public void Continue() {
      Stmt("continue");
    }

    public void EndCase() {
      Break();
      EndScope();
    }

    public CodeBuilder BeginDefault() {
      return BeginScope($"default:");
    }

    public void EndDefault() {
      Break();
      EndScope();
    }

    public void Break() {
      Stmt("break");
    }

    public void Attr<T>(params object[] args) where T : Attribute {
      Line($"[{typeof(T).FullName}({string.Join(", ", args.Select(ToStringAttrArg).ToArray())})]");
    }

    public void Attr(string name, params object[] args) {
      Line($"[{name}({string.Join(", ", args.Select(ToStringAttrArg).ToArray())})]");
    }

    public void Return(string expression) {
      Stmt($"return {expression}");
    }

    public void EndScope() {
      --_scope;
      Line("}");
    }

    public void Comment(string msg) {
      Line($"// {msg}");
    }

    public void Stmt(string line) {
      Line(line + ";");
    }

    public void Var(string name, object init = null, string type = "var") {
      if (init == null) {
        Stmt($"{type} {name}");
      } else {
        Stmt($"{type} {name} = {init}");
      }
    }

    public void Using(string ns) {
      Line($"using {ns};");
    }

    public void Line(string line) {
      _sb.AppendLine(IndentString + line);
    }

    public void DefIf(string expression) {
      _sb.AppendLine($"#if {expression}");
    }

    public void DefElse() {
      _sb.AppendLine($"#else");
    }

    public void DefEndIf() {
      _sb.AppendLine($"#endif");
    }

    public void LineBreak() {
      _sb.AppendLine();
    }

    public (string, string)[] Args(params (string, string)[] args) {
      return args;
    }

    public void Const(string typ, string name, object value, string modifiers = "public") {
      Field(typ, name, modifiers + " const", value);
    }

    public void Field(string typ, string name, string modifiers = "public", object init = null) {
      if (init == null) {
        Line($"{AddSpace(modifiers)}{typ} {name};");
      } else {
        Line($"{AddSpace(modifiers)}{typ} {name} = {init};");
      }
    }

    public override string ToString() {
      return _sb.ToString();
    }

    public StringExpr Expr(string expr) {
      return new StringExpr(expr);
    }

    string IndentString => new string(' ', _scope * INDENT);

    static string AddSpace(string value) {
      if (string.IsNullOrEmpty(value)) {
        return "";
      }

      return value + " ";
    }

    static string ToStringAttrArg(object obj) {
      switch (obj) {
        case System.Enum e:
          return $"{e.GetType().Name}.{obj}";

        case string s:
          return '"' + s + '"';

        case float f:
          return $"{f}f";

        case StringExpr x:
          return x.Value;

        default:
          return obj.ToString();
      }
    }

    public void Dispose() {
      EndScope();
    }
  }
}