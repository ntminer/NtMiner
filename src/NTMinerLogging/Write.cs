using System;

namespace NTMiner {
    public static class Write {
        public static Action<string, ConsoleColor, bool> WriteUserLineMethod;
        public static Action<string, ConsoleColor> WriteDevLineMethod;

        static Write() {
            WriteUserLineMethod = (line, color, isNotice) => {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line, isNotice);
                Console.ForegroundColor = oldColor;
            };
            WriteDevLineMethod = (line, color) => {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = oldColor;
            };
        }

        public static void UserLine(string text, ConsoleColor foreground, bool isNotice = true) {
            WriteUserLineMethod?.Invoke(text, foreground, isNotice);
        }

        public static void DevLine(string text) {
            if (!DevMode.IsDevMode) {
                return;
            }
            text = $"{DateTime.Now.ToString("HH:mm:ss fff")}  {text}";
            WriteDevLineMethod?.Invoke(text, ConsoleColor.White);
        }

        public static void DevLine(string text, ConsoleColor foreground) {
            if (!DevMode.IsDevMode) {
                return;
            }
            text = $"{DateTime.Now.ToString("HH:mm:ss fff")}  {text}";
            WriteDevLineMethod?.Invoke(text, foreground);
        }
    }
}
