using NTWebSocket;
using System.Net;

namespace NTMiner {
    public static class NTWebSocketServer {
        private static IWebSocketServer _server;
        public static void Start() {
            _server = WebSocketServer.Create(new ServerConfig {
                Scheme = SchemeType.ws,
                Ip = IPAddress.Any,
                Port = NTKeyword.MinerClientPort + 1000
            });
            _server.OnMessage = (conn, message) => {
                conn.Send("Echo:" + message);
            };
            _server.Start();
        }

        public static void Stop() {
            _server?.Dispose();
        }
    }
}
