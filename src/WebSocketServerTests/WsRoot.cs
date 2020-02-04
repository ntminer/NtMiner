using System;
using WebSocketSharp.Server;

namespace NTMiner {
    public class WsRoot {
        static void Main() {
            DevMode.SetDevMode();

            var wssv = new WebSocketServer("ws://0.0.0.0:8088");
            wssv.Log.Level = WebSocketSharp.LogLevel.Trace;
            wssv.AddWebSocketService<AllInOneBehavior>("/");
            wssv.Start();
            VirtualRoot.StartTimer();
            Windows.ConsoleHandler.Register(wssv.Stop);
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
