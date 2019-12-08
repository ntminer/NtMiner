using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class DevMode {
        public static bool IsDevMode { get; private set; }
        public static bool IsInUnitTest { get; private set; }

        public static void SetDevMode() {
            IsDevMode = true;
        }

        static DevMode() {
            List<string> _commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
#if DEBUG
            // 如果编译时是DEBUG则是Debug
            IsDevMode = true;
#else
            // 如果启动时追加了--debug或--dev参数则是Debug
            IsDevMode = 
                _commandLineArgs.Contains("--debug", StringComparer.OrdinalIgnoreCase) 
             || _commandLineArgs.Contains("--dev", StringComparer.OrdinalIgnoreCase);
#endif
            // 如果是Debug但追加了--release参数则不是Debug
            if (IsDevMode && _commandLineArgs.Contains("--release", StringComparer.OrdinalIgnoreCase)) {
                IsDevMode = false;
            }
            // 如果没有启动入口程序集则是UnitTest
            if (Assembly.GetEntryAssembly() == null) {
                IsInUnitTest = true;
            }
        }
    }
}
