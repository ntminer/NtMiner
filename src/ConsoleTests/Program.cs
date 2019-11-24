using NTWebSocket;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NTMiner {
    class Program {
        static void Main() {
            DevMode.SetDevMode();

            WebSocketTest();

            Console.ReadKey();
        }

        static void WebSocketTest() {
            using (var server = WebSocketServer.Create(
                new ServerConfig {
                    Scheme = SchemeType.ws,
                    Ip = IPAddress.Parse("0.0.0.0"),
                    Port = 8088
            })) {
                server.OnOpen = (conn) => {
                    Write.DevDebug("OnOpen: opened");
                    Write.DevWarn("ConnCount " + server.ConnCount);
                };
                server.OnClose = (conn) => {
                    Write.DevDebug("OnClose: closed");
                    Write.DevWarn("ConnCount " + server.ConnCount);
                };
                server.OnPing = (conn, data) => {
                    conn.SendPong(data);
                };
                server.OnPong = (conn, data) => {
                    Write.DevDebug("OnPong: " + Encoding.UTF8.GetString(data));
                };
                server.OnError = (conn, e) => {
                    Write.DevException(e);
                };
                server.OnMessage = (conn, message) => {
                    if (string.IsNullOrEmpty(message)) {
                        return;
                    }
                    JsonRequest request = VirtualRoot.JsonSerializer.Deserialize<JsonRequest>(message);
                    if (request == null) {
                        return;
                    }
                    switch (request.action) {
                        case "getSpeed":
                            Dictionary<string, object> data = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(request.json);
                            string messageId = string.Empty;
                            if (data != null) {
                                messageId = data["messageId"]?.ToString();
                            }
                            conn.Send(new JsonResponse {
                                messageId = messageId,
                                code = 200,
                                phrase = "Ok",
                                des = "成功",
                                res = "getSpeed",
                                data = new Dictionary<string, object> {
                                        {"str", "hello" },
                                        {"num", 111 },
                                        {"date", DateTime.Now }
                                    }
                            }.ToJson());
                            break;
                        default:
                            conn.Send(message);
                            break;
                    }
                    Write.DevWarn("ConnCount " + server.ConnCount);
                };
                server.Start();


                var input = Console.ReadLine();
                while (input != "exit") {
                    input = Console.ReadLine();
                }
            }
        }
    }
}
