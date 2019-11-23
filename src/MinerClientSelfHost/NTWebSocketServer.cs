using NTWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NTMiner {
    public static class NTWebSocketServer {
        private static IWebSocketServer _server;
        public static void Start() {
            Dictionary<Guid, IWebSocketConnection> connDic = new Dictionary<Guid, IWebSocketConnection>();
            _server = ServerFactory.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = NTKeyword.MinerClientPort + 1000
            });
            _server.Start(conn => {
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
                            var speedData = Report.CreateSpeedData();
                            string json = VirtualRoot.JsonSerializer.Serialize(speedData);
                            conn.Send("result of getSpeed:" + json);
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
