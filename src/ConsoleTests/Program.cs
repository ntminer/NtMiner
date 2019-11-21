using NTWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();

            WebSocketTest();

            Console.ReadKey();
        }
        
        static void WebSocketTest() {
            Dictionary<Guid, IWebSocketConnection> connDic = new Dictionary<Guid, IWebSocketConnection>();
            var server = new WebSocketServer($"ws://0.0.0.0:8088");
            server.Start(socket => {
                socket.OnOpen = () => {
                    string id = socket.ConnectionInfo.Id.ToString();
                    connDic.Add(socket.ConnectionInfo.Id, socket);
                    socket.Send($"clientId:" + id);
                };
                socket.OnClose = () => {
                    connDic.Remove(socket.ConnectionInfo.Id);
                };
                socket.OnMessage = message => {
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
                            if (connDic.TryGetValue(clientId, out IWebSocketConnection conn)) {
                                conn.Send("result of getSpeed:{'a':'this is a test'}");
                            }
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
