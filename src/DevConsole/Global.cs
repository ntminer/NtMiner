using System;

namespace NTMiner {
    public static class Global {
        public static readonly Logger Logger = new Logger();
    }

    public class Logger {
        public void ErrorDebugLine(string message, Exception e) {
            Console.WriteLine(message);
            Console.WriteLine(e.StackTrace);
        }
    }
}
