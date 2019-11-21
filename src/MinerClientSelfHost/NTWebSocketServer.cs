using NTWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NTMiner {
    public static class NTWebSocketServer {
        private static WebSocketServer _server;
        public static void Start() {
            Dictionary<Guid, IWebSocketConnection> connDic = new Dictionary<Guid, IWebSocketConnection>();
            _server = new WebSocketServer(scheme: WebSocketScheme.ws, ip: IPAddress.Parse("0.0.0.0"), port: NTKeyword.MinerClientPort + 1000);
            _server.Start(socket => {
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
                                var speedData = Report.CreateSpeedData();
                                string json = VirtualRoot.JsonSerializer.Serialize(speedData);
                                conn.Send("result of getSpeed:" + json);
                            }
                            break;
                        default:
                            connDic.Values.ToList().ForEach(s => s.Send("Echo:" + command));
                            break;
                    }
                };
            });
        }

        public static void Stop() {
            _server?.Dispose();
        }
    }
}
