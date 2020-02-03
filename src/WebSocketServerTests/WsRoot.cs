using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace NTMiner {
    public class WsRoot {
        static void Main() {
            DevMode.SetDevMode();

            VirtualRoot.AddCmdPath<GetSpeedWsCommand>(action: message => {
                message.Sessions.SendToAsync(new JsonResponse(message.MessageId) {
                    code = 200,
                    phrase = "Ok",
                    des = "成功",
                    action = GetSpeedWsCommand.Action,
                    data = new Dictionary<string, object> {
                                        {"str", "hello" },
                                        {"num", 111 },
                                        {"date", DateTime.Now }
                                    }
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
