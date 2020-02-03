using System;
using WebSocketSharp.Server;
using WsCommands;

namespace NTMiner {
    public class WsRoot {
        static void Main() {
            DevMode.SetDevMode();

            VirtualRoot.AddCmdPath<GetSpeedWsCommand>(action: message => {
                message.Sessions.SendToAsync(new WsMessage {
                    action = GetSpeedWsCommand.RequestAction
                }.ToJson(), message.SessionId, completed: null);
            }, typeof(WsRoot), logType: LogEnum.None);

            var wssv = new WebSocketServer("ws://0.0.0.0:8088");
            wssv.Log.Level = WebSocketSharp.LogLevel.Trace;
            wssv.AddWebSocketService<AllInOne>("/");
            wssv.Start();
            VirtualRoot.StartTimer();
            Windows.ConsoleHandler.Register(wssv.Stop);
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
