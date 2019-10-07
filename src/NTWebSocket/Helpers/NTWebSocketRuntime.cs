using System;

namespace NTWebSocket.Helpers {
    internal static class NTWebSocketRuntime {
        public static bool IsRunningOnMono() {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static bool IsRunningOnWindows() {
            return true;
        }
    }
}