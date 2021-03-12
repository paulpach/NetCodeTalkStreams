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
using System.Diagnostics;

namespace Transport {
  public static class Log {
    static object sync;
    static Action<string> infoCallback;
    static Action<string> warnCallback;
    static Action<string> errorCallback;
    static Action<Exception> exnCallback;

    public static void InitForConsole() {
      Init(
        info => {
          Console.ForegroundColor = ConsoleColor.Gray;
          Console.WriteLine(info);
          Console.ForegroundColor = ConsoleColor.Gray;
        },

        warn => {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine(warn);
          Console.ForegroundColor = ConsoleColor.Gray;
        },

        error => {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(error);
          Console.ForegroundColor = ConsoleColor.Gray;
        },

        exn => {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(exn.Message);
          Console.WriteLine(exn.StackTrace);
          Console.ForegroundColor = ConsoleColor.Gray;
        }
      );
    }

    public static void Init(Action<string> info, Action<string> warn, Action<string> error, Action<Exception> exn) {
      sync = new object();
      infoCallback = info;
      warnCallback = warn;
      errorCallback = error;
      exnCallback = exn;
    }

    public static bool Initialized {
      get => sync != null;
    }

    public static void Write(string fmt, params object[] args) {
      if (infoCallback != null) {
        lock (sync) {
          infoCallback(string.Format(fmt, args));
        }
      }
    }

    [Conditional("TRACE")]
    public static void Trace(object value) {
      Info(value);
    }

    [Conditional("TRACE")]
    public static void Trace(string fmt, params object[] args) {
      Info(fmt, args);
    }
    
    [Conditional("TRACE")]
    public static void TraceError(object value) {
      Error(value);
    }

    [Conditional("TRACE")]
    public static void TraceError(string fmt, params object[] args) {
      Error(fmt, args);
    }
    
    [Conditional("TRACE")]
    public static void TraceWarn(object value) {
      Warn(value);
    }

    [Conditional("TRACE")]
    public static void TraceWarn(string fmt, params object[] args) {
      Warn(fmt, args);
    }
    
    [Conditional("DEBUG")]
    public static void Debug(object value) {
      Info(value);
    }

    [Conditional("DEBUG")]
    public static void Debug(string fmt, params object[] args) {
      Info(fmt, args);
    }
    
    [Conditional("DEBUG")]
    public static void DebugError(object value) {
      Error(value);
    }

    [Conditional("DEBUG")]
    public static void DebugError(string fmt, params object[] args) {
      Error(fmt, args);
    }
    
    [Conditional("DEBUG")]
    public static void DebugWarn(object value) {
      Warn(value);
    }

    [Conditional("DEBUG")]
    public static void DebugWarn(string fmt, params object[] args) {
      Warn(fmt, args);
    }
    
    public static void Info(object value) {
      if (infoCallback != null) {
        lock (sync) {
          infoCallback(value == null ? "NULL" : value.ToString());
        }
      }
    }
    
    public static void Info(string fmt, params object[] args) {
      if (infoCallback != null) {
        lock (sync) {
          infoCallback(string.Format(fmt, args));
        }
      }
    }
    
    public static void Warn(string fmt, params object[] args) {
      if (warnCallback != null) {
        lock (sync) {
          warnCallback(string.Format(fmt, args));
        }
      }
    }
    
    public static void Warn(object value) {
      if (warnCallback != null) {
        lock (sync) {
          warnCallback(value == null ? "NULL" : value.ToString());
        }
      }
    }

    public static void Error(string fmt, params object[] args) {
      if (errorCallback != null) {
        lock (sync) {
          errorCallback(string.Format(fmt, args));
        }
      }
    }

    public static void Error(object value) {
      if (errorCallback != null) {
        lock (sync) {
          errorCallback(value == null ? "NULL" : value.ToString());
        }
      }
    }

    public static void Exception(Exception exn) {
      if (exnCallback != null) {
        lock (sync) {
          exnCallback(exn);
        }
      }
    }
  }
}
