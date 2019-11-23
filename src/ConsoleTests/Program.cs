using NTWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NTMiner {
    class Program {
        static void Main() {
            DevMode.SetDevMode();

            WebSocketTest();

            Console.ReadKey();
        }

        static void WebSocketTest() {
            Dictionary<Guid, IWebSocketConnection> connDic = new Dictionary<Guid, IWebSocketConnection>();
            using (var server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.wss,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = 8088
            })) {
                server.Start(conn => {
                    conn.OnOpen = () => {
                        string id = conn.ConnectionInfo.Id.ToString();
                        connDic.Add(conn.ConnectionInfo.Id, conn);
                        conn.Send($"clientId:" + id);
                    };
                    conn.OnClose = () => {
                        connDic.Remove(conn.ConnectionInfo.Id);
                    };
                    conn.OnMessage = message => {
                        if (string.IsNullOrEmpty(message)) {
                            return;
                        }
                        string[] parts = message.Split(':');
                        if (parts.Length < 2) {
                            return;
                        }
                        if (!Guid.TryParse(parts[0], out Guid clientId) || !connDic.ContainsKey(clientId)) {
                            return;
                        }
                        string command = parts[1];
                        switch (command) {
                            case "getSpeed":
                                conn.Send("result of getSpeed:{'a':'this is a test'}");
                                break;
                            default:
                                connDic.Values.ToList().ForEach(s => s.Send("Echo:" + command));
                                break;
                        }
                    };
                });


                var input = Console.ReadLine();
                while (input != "exit") {
                    foreach (var socket in connDic.Values.ToList()) {
                        socket.Send(input);
                    }
                    input = Console.ReadLine();
                }
            }
        }
    }
}
