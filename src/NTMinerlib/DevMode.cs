using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class DevMode {
        private const string AppDebugParamName = "--debug";
        private const string AppReleaseParamName = "--release";

        public static bool IsDevMode { get; private set; }

        static DevMode() {
            List<string> _commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
            IsDevMode = _commandLineArgs.Contains(AppDebugParamName, StringComparer.OrdinalIgnoreCase);
#if DEBUG
            IsDevMode = true;
#endif
            if (IsDevMode && _commandLineArgs.Contains(AppReleaseParamName, StringComparer.OrdinalIgnoreCase)) {
                IsDevMode = false;
            }
        }
    }
}
