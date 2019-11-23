using NTWebSocket;
using System;
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
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8088
            })) {
                server.Start(conn => {
                    conn.OnOpen = () => {
                        Write.DevDebug("OnOpen: opened");
                        Write.DevWarn("ConnCount " + server.ConnCount);
                    };
                    conn.OnClose = () => {
                        Write.DevDebug("OnClose: closed");
                        Write.DevWarn("ConnCount " + server.ConnCount);
                    };
                    conn.OnPing = (data) => {
                        conn.SendPong(data);
                    };
                    conn.OnPong = (data) => {
                        Write.DevDebug("OnPong: " + Encoding.UTF8.GetString(data));
                    };
                    conn.OnError = e => {
                        Write.DevException(e);
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
                        Write.DevWarn("ConnCount " + server.ConnCount);
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
