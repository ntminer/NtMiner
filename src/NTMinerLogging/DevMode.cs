using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class DevMode {
        private const string AppDebugParamName = "--debug";
        private const string AppReleaseParamName = "--release";
        private const string AppDevParamName = "--dev";

        public static bool IsDebugMode { get; private set; }
        public static bool IsDevMode { get; private set; }
        public static bool IsInUnitTest { get; private set; }

        public static void SetDevMode() {
            IsDebugMode = true;
            IsDevMode = true;
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
            if (Assembly.GetEntryAssembly() == null) {
                IsInUnitTest = true;
            }
        }
    }
}
