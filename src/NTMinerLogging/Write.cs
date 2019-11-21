using System;
using System.Threading;

namespace NTMiner {
    public static class Write {
        private static readonly ThreadLocal<NTStopwatch> _stopwatch = new ThreadLocal<NTStopwatch>(() => {
            return new NTStopwatch();
        });
        public static NTStopwatch Stopwatch {
            get {
                return _stopwatch.Value;
            }
        }

        private static readonly Action<string, ConsoleColor> _consoleUserLineMethod = (line, color) => {
            InitOnece();
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = oldColor;
        };
        private static Action<string, ConsoleColor> UserLineMethod = _consoleUserLineMethod;

        private static readonly object _locker = new object();
        private static bool _isInited = false;
        private static void InitOnece() {
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        _isInited = true;
                        NTMinerConsole.Alloc();
                    }
                }
            }
        }

        public static void SetUserLineMethod(Action<string, ConsoleColor> action) {
            UserLineMethod = action;
        }

        public static void UserLine(string text, MessageType messageType = MessageType.Default) {
            UserLine($"NTMiner {messageType.ToString().PadLeft(10)}  {text}", messageType.ToConsoleColor());
        }

        public static void UserLine(string text, string messageType, ConsoleColor color) {
            UserLine($"NTMiner {messageType.PadLeft(10)}  {text}", color);
        }

        public static void UserError(string text) {
            UserLine(text, MessageType.Error);
        }

        public static void UserInfo(string text) {
            UserLine(text, MessageType.Info);
        }

        public static void UserOk(string text) {
            UserLine(text, MessageType.Ok);
        }

        public static void UserWarn(string text) {
            UserLine(text, MessageType.Warn);
        }

        public static void UserFail(string text) {
            UserLine(text, MessageType.Fail);
        }

        public static void UserLine(string text, ConsoleColor foreground) {
            UserLineMethod?.Invoke(text, foreground);
        }

        public static void DevLine(string text, MessageType messageType = MessageType.Default) {
            if (!DevMode.IsDevMode) {
                return;
            }
            InitOnece();
            text = $"{DateTime.Now.ToString("HH:mm:ss fff")}  {messageType.ToString()} {text}";
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = messageType.ToConsoleColor();
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public static void DevException(Exception e) {
            DevLine(e.GetInnerMessage() + e.StackTrace, MessageType.Error);
        }

        public static void DevException(string message, Exception e) {
            DevLine(message + e.StackTrace, MessageType.Error);
        }

        public static void DevError(string text) {
            DevLine(text, MessageType.Error);
        }

        public static void DevDebug(string text) {
            DevLine(text, MessageType.Debug);
        }

        public static void DevOk(string text) {
            DevLine(text, MessageType.Ok);
        }

        public static void DevWarn(string text) {
            DevLine(text, MessageType.Warn);
        }

        public static void DevTimeSpan(string text) {
            DevLine(text, MessageType.TimeSpan);
        }

        public static void DevFail(string text) {
            DevLine(text, MessageType.Fail);
        }
    }
}
