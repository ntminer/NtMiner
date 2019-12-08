using NTMiner.Impl;
using System;

namespace NTMiner {
    public static class Logger {
        private static string s_logDir;
        public static string Dir {
            get { return s_logDir; }
        }

        public static void SetDir(string fullPath) {
            s_logDir = fullPath;
        }

        private static bool _isEnabled = true;
        public static void Enable() {
            _isEnabled = true;
        }

        /// <summary>
        /// 禁用Logger则可以避免行走到Logger逻辑中去，从而避免创建出log文件夹和文件
        /// </summary>
        public static void Disable() {
            _isEnabled = false;
        }

        // 因为初始化Log4NetLoggingService时会用到Dir所以需要延迟初始化避免静态构造失败
        private static readonly Lazy<ILoggingService> _logger = new Lazy<ILoggingService>(() => new Log4NetLoggingService());

        public static void Debug(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.Debug(message);
        }
        public static void InfoDebugLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.InfoDebugLine(message);
        }
        public static void OkDebugLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.OkDebugLine(message);
        }
        public static void WarnDebugLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.WarnDebugLine(message);
        }
        public static void ErrorDebugLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.ErrorDebugLine(message);
        }
        public static void ErrorDebugLine(Exception exception) {
            if (!_isEnabled) {
                return;
            }
            if (exception == null) {
                return;
            }
            ErrorDebugLine(exception.GetInnerMessage(), exception);
        }
        public static void ErrorDebugLine(object message, Exception exception) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.ErrorDebugLine(message, exception);
        }

        public static void OkWriteLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.OkWriteLine(message);
        }
        public static void WarnWriteLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.WarnWriteLine(message);
        }
        public static void ErrorWriteLine(object message) {
            if (!_isEnabled) {
                return;
            }
            _logger.Value.ErrorWriteLine(message);
        }
    }
}
