using System;
using System.Collections.Generic;
using WebSocketSharp;
using WsCommands;

namespace NTMiner {
    class Program {
        static void Main() {
            using (var ws = new WebSocket("ws://localhost:8088/")) {
                ws.OnOpen += (sender, e) => {
                    Write.UserWarn($"WebSocket Open");
                    ws.Send("Hi!");
                };
                ws.OnMessage += (sender, e) => {
                    if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                        return;
                    }
                    JsonRequest request = VirtualRoot.JsonSerializer.Deserialize<JsonRequest>(e.Data);
                    if (request == null) {
                        return;
                    }
                    switch (request.action) {
                        case GetSpeedWsCommand.RequestAction:
                            request.Parse(out Guid messageId);
                            ws.SendAsync(new JsonResponse(messageId) {
                                code = 200,
                                phrase = "Ok",
                                des = "成功",
                                action = GetSpeedWsCommand.ResponseAction,
                                data = new Dictionary<string, object> {
                                        {"str", "hello" },
                                        {"num", 111 },
                                        {"date", DateTime.Now }
                                    }
                            }.ToJson(), completed: null);
                            break;
                        default:
                            Write.UserInfo(e.Data);
                            break;
                    }
                };
                ws.OnError += (sender, e) => {
                    Write.UserError(e.Message);
                };
                ws.OnClose += (sender, e) => {
                    Write.UserWarn($"WebSocket Close {e.Code} {e.Reason}");
                };
                ws.Log.Level = LogLevel.Trace;
                ws.Connect();
                Windows.ConsoleHandler.Register(ws.Close);
                Console.WriteLine("\nType 'exit' to exit.\n");
                while (true) {
                    Console.Write("> ");
                    var action = Console.ReadLine();
                    if (action == "exit") {
                        break;
                    }

                    if (!ws.IsAlive) {
                        ws.Connect();
                    }
                    switch (action) {
                        case "getSpeed":
                            ws.Send(VirtualRoot.JsonSerializer.Serialize(new JsonRequest("getSpeed", json: string.Empty)));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
