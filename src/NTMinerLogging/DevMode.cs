using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class DevMode {
        private const string AppDebugParamName = "--debug";
        private const string AppReleaseParamName = "--release";
        private const string AppDevParamName = "--dev";

        public static bool IsDebugMode { get; private set; }
        public static bool IsDevMode { get; private set; }

        // 为了在设计视图避开日志控制台窗口
        public static void SetIsDevMode(bool value) {
            IsDevMode = value;
        }

        static DevMode() {
            List<string> _commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
            IsDebugMode = _commandLineArgs.Contains(AppDebugParamName, StringComparer.OrdinalIgnoreCase);
#if DEBUG
            IsDebugMode = true;
#endif
            if (IsDebugMode && _commandLineArgs.Contains(AppReleaseParamName, StringComparer.OrdinalIgnoreCase)) {
                IsDebugMode = false;
            }
            IsDevMode = IsDebugMode;
            if (!IsDevMode && _commandLineArgs.Contains(AppDevParamName, StringComparer.OrdinalIgnoreCase)) {
                IsDevMode = true;
            }
        }
    }
}
