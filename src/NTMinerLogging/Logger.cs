using NTMiner.Logging;
using System;

namespace NTMiner {
    public static class Logger {
        private static readonly ILoggingService _logger = new Log4NetLoggingService();
        public static void Debug(object message) {
            _logger.Debug(message);
        }
        public static void DebugFormatted(string format, params object[] args) {
            _logger.DebugFormatted(format, args) ;
        }
        public static void InfoDebugLine(object message) {
            _logger.InfoDebugLine(message);
        }
        public static void InfoDebugLineFormatted(string format, params object[] args) {
            _logger.InfoDebugLineFormatted(format, args);
        }
        public static void OkDebugLine(object message) {
            _logger.OkDebugLine(message);
        }
        public static void WarnDebugLine(object message) {
            _logger.WarnDebugLine(message);
        }
        public static void WarnDebugLine(object message, Exception exception) {
            _logger.WarnDebugLine(message, exception);
        }
        public static void WarnDebugLineFormatted(string format, params object[] args) {
            _logger.WarnDebugLineFormatted(format, args);
        }
        public static void ErrorDebugLine(object message) {
            _logger.ErrorDebugLine(message);
        }
        public static void ErrorDebugLine(object message, Exception exception) {
            _logger.ErrorDebugLine(message, exception);
        }
        public static void ErrorDebugLineFormatted(string format, params object[] args) {
            _logger.ErrorDebugLineFormatted(format, args);
        }
        public static void FatalDebugLine(object message) {
            _logger.FatalDebugLine(message);
        }
        public static void FatalDebugLine(object message, Exception exception) {
            _logger.FatalDebugLine(message, exception);
        }
        public static void FatalDebugLineFormatted(string format, params object[] args) {
            _logger.FatalDebugLineFormatted(format, args);
        }


        public static void OkWriteLine(object message) {
            _logger.OkWriteLine(message);
        }
        public static void WarnWriteLine(object message) {
            _logger.WarnWriteLine(message);
        }
        public static void ErrorWriteLine(object message) {
            _logger.ErrorWriteLine(message);
        }
    }
}
