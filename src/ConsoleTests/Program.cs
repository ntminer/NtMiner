using NTWebSocket;
using System;
using System.Net;

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
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8088
            })) {
                server.Start(conn => {
                    conn.OnOpen = () => {
                        Write.DevDebug("opened");
                    };
                    conn.OnClose = () => {
                        Write.DevDebug("closed");
                    };
                    conn.OnMessage = message => {
                        if (string.IsNullOrEmpty(message)) {
                            return;
                        }
                        switch (message) {
                            case "getSpeed":
                                conn.Send("result of getSpeed:{'a':'this is a test'}");
                                break;
                            default:
                                conn.Send("Echo:" + message);
                                break;
                        }
                    };
                });


                var input = Console.ReadLine();
                while (input != "exit") {
                    input = Console.ReadLine();
                }
            }
        }
    }
}
