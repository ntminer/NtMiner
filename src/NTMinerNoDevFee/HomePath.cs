using System;

namespace NTMiner {
    public static class HomePath {
        public static string HomeDirFullName { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
    }
}
