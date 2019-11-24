using System;
using WebSocketSharp.Server;

namespace NTMiner {
    class Program {
        static void Main() {
            DevMode.SetDevMode();

            var wssv = new WebSocketServer("ws://0.0.0.0:8088");
            wssv.Log.Level = WebSocketSharp.LogLevel.Trace;
            wssv.AddWebSocketService<Echo>("/Echo");
            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
