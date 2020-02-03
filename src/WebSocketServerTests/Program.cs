using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace NTMiner {
    class Program {
        static void Main() {
            DevMode.SetDevMode();

            VirtualRoot.AddCmdPath<GetSpeedWsCommand>(action: message => {
                message.Ws.Send(new JsonResponse(message.Id) {
                    code = 200,
                    phrase = "Ok",
                    des = "成功",
                    action = message.Action,
                    data = new Dictionary<string, object> {
                                        {"str", "hello" },
                                        {"num", 111 },
                                        {"date", DateTime.Now }
                                    }
                }.ToJson());
            }, typeof(Program), logType: LogEnum.None);

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
