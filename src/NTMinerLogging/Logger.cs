using NTMiner.Logging;
using System;

namespace NTMiner {
    public static class Logger {
        private static readonly ILoggingService s_logger = new Log4NetLoggingService();
        public static void Debug(object message) {
            s_logger.Debug(message);
        }
        public static void InfoDebugLine(object message) {
            s_logger.InfoDebugLine(message);
        }
        public static void OkDebugLine(object message) {
            s_logger.OkDebugLine(message);
        }
        public static void WarnDebugLine(object message) {
            s_logger.WarnDebugLine(message);
        }
        public static void WarnDebugLine(object message, Exception exception) {
            s_logger.WarnDebugLine(message, exception);
        }
        public static void ErrorDebugLine(object message) {
            s_logger.ErrorDebugLine(message);
        }
        public static void ErrorDebugLine(object message, Exception exception) {
            s_logger.ErrorDebugLine(message, exception);
        }
        public static void FatalDebugLine(object message) {
            s_logger.FatalDebugLine(message);
        }
        public static void FatalDebugLine(object message, Exception exception) {
            s_logger.FatalDebugLine(message, exception);
        }

        public static void OkWriteLine(object message) {
            s_logger.OkWriteLine(message);
        }
        public static void WarnWriteLine(object message) {
            s_logger.WarnWriteLine(message);
        }
        public static void ErrorWriteLine(object message) {
            s_logger.ErrorWriteLine(message);
        }
    }
}
