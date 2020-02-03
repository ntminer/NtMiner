using System;
using WebSocketSharp.Server;

namespace NTMiner {
    class Program {
        static void Main() {
            DevMode.SetDevMode();

            var wssv = new WebSocketServer("ws://0.0.0.0:8088");
            wssv.Log.Level = WebSocketSharp.LogLevel.Trace;
            wssv.AddWebSocketService<AllInOne>("/");
            wssv.Start();
            Windows.ConsoleHandler.Register(wssv.Stop);
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
