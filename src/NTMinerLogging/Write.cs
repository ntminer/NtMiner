using System;

namespace NTMiner {
    public static class Write {
        private static readonly Action<string, ConsoleColor, bool> _writeUserLineMethod;
        public static Action<string, ConsoleColor, bool> WriteUserLineMethod;

        static Write() {
            if (DevMode.IsDevMode && !System.Diagnostics.Debugger.IsAttached) {
                ConsoleManager.Show();
            }
            _writeUserLineMethod = (line, color, isNotice) => {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line, isNotice);
                Console.ForegroundColor = oldColor;
            };
            WriteUserLineMethod = _writeUserLineMethod;
        }

        public static void ResetWriteUserLineMethod() {
            WriteUserLineMethod = _writeUserLineMethod;
        }

        public static void UserLine(string text, MessageType messageType = MessageType.Default) {
            UserLine($"{messageType.ToString()} {text}", messageType.ToConsoleColor());
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

        public static void UserFatal(string text) {
            UserLine(text, MessageType.Fatal);
        }

        public static void UserLine(string text, ConsoleColor foreground, bool isNotice = true) {
            WriteUserLineMethod?.Invoke(text, foreground, isNotice);
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
