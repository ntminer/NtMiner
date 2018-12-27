using NTMiner.Logging;
using System;
using System.IO;

namespace NTMiner {
    public static class Global {
        public static readonly ILoggingService Logger = new Log4NetLoggingService();

        private static string _sha1 = null;
        public static string Sha1 {
            get {
                if (_sha1 == null) {
                    _sha1 = CryptoUtil.Sha1(File.ReadAllBytes(ClientId.AppFileFullName));
                }
                return _sha1;
            }
        }

        public static void DebugLine(string text) {
            text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {text}";
            Console.WriteLine(text);
        }

        public static void WriteLine(string text) {
            text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {text}";
            Console.WriteLine(text);
            Logger.Info(text);
        }
    }
}
