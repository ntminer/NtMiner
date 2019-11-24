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
            using (var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Any,
                Port = 8088
            })) {
                server.Start(onOpen: (conn) => {
                    Write.DevDebug("OnOpen: opened");
                    Write.DevWarn("ConnCount " + server.ConnCount);
                },
                onClose: (conn) => {
                    Write.DevDebug("OnClose: closed");
                    Write.DevWarn("ConnCount " + server.ConnCount);
                },
                onPing: (conn, data) => {
                    conn.SendPong(data);
                },
                onPong: (conn, data) => {
                    Write.DevDebug("OnPong: " + Encoding.UTF8.GetString(data));
                },
                onError: (conn, e) => {
                    Write.DevException(e);
                },
                onMessage: (conn, message) => {
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
                });

                var input = Console.ReadLine();
                while (input != "exit") {
                    input = Console.ReadLine();
                }
            }
        }
    }
}
