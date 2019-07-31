using System;

namespace NTMiner {
    public static class Write {
        private static readonly Action<string, ConsoleColor> _consoleUserLineMethod = (line, color) => {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = oldColor;
        };
        public static Action<string, ConsoleColor> UserLineMethod = _consoleUserLineMethod;

        private static readonly object _locker = new object();
        private static bool _isInited = false;
        public static void  Init() {
            if (!_isInited) {
                lock (_locker) {
                    if (!_isInited) {
                        _isInited = true;
                        if (DevMode.IsDevMode) {
                            NTMinerConsole.Show();
                        }
                    }
                }
            }
        }

        public static void SetConsoleUserLineMethod() {
            UserLineMethod = _consoleUserLineMethod;
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

        public static void UserEvent(string text) {
            UserLine(text, MessageType.Event);
        }

        public static void UserFail(string text) {
            UserLine(text, MessageType.Fail);
        }

        public static void UserFatal(string text) {
            UserLine(text, MessageType.Fatal);
        }

        public static void UserLine(string text, ConsoleColor foreground) {
            UserLineMethod?.Invoke(text, foreground);
        }

        public static void DevLine(string text, MessageType messageType = MessageType.Default) {
            if (!DevMode.IsDevMode) {
                return;
            }
            text = $"{DateTime.Now.ToString("HH:mm:ss fff")}  {messageType.ToString()} {text}";
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = messageType.ToConsoleColor();
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
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

        public static void DevFail(string text) {
            DevLine(text, MessageType.Fail);
        }

        public static void DevFatal(string text) {
            DevLine(text, MessageType.Fatal);
        }
    }
}
