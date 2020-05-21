using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    /// <summary>
    /// 为什么识别和区分开发者模式，因为开发者模式和用户模式看到的界面会不一样。开发者模式和
    /// 用户模式使用的数据库也不一样，开发者模式使用的litedb，用户模式使用的磁盘上的json文件，
    /// 还有就是开发者模式时前缀为Dev的Writer的方法比如NTMinerConsole.DevDebug行动，非开发者模式时静默。
    /// </summary>
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
