using NTMiner.Logging;
using System;
using System.IO;

namespace NTMiner {
    public static class Global {
        public static string LogDir;
        public static readonly ILoggingService Logger = new Log4NetLoggingService();

        public const string Localhost = "localhost";
        public static int ClientPort {
            get {
                return 3336;
            }
        }

        private static string _sha1 = null;
        public static string Sha1 {
            get {
                if (_sha1 == null) {
                    _sha1 = HashUtil.Sha1(File.ReadAllBytes(ClientId.AppFileFullName));
                }
                return _sha1;
            }
        }

        public static void DebugLine(string text, ConsoleColor foreground) {
            if (!DevMode.IsDevMode) {
                return;
            }
            text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {text}";
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteLine(string text, ConsoleColor foreground) {
            text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {text}";
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }
    }
}
