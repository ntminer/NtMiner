using NTWebSocket;
using System.Net;

namespace NTMiner {
    public static class NTWebSocketServer {
        private static IWebSocketServer _server;
        public static void Start() {
            _server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Parse("0.0.0.0"),
                Port = NTKeyword.MinerClientPort + 1000
            });
            _server.Start(conn => {
                conn.OnMessage = message => {
                    if (string.IsNullOrEmpty(message)) {
                        return;
                    }
                    switch (message) {
                        case "getSpeed":
                            var speedData = Report.CreateSpeedData();
                            string json = VirtualRoot.JsonSerializer.Serialize(speedData);
                            conn.Send("result of getSpeed:" + json);
                            break;
                        default:
                            conn.Send("Echo:" + message);
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
