using NTWebSocket;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class NTWebSocketServer {
        private static WebSocketServer _server;
        public static void Start() {
            var allSockets = new List<IWebSocketConnection>();
            _server = new WebSocketServer($"ws://0.0.0.0:{NTKeyword.MinerClientPort + 1000}");
            _server.Start(socket => {
                socket.OnOpen = () => {
                    Write.DevDebug("Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () => {
                    Write.DevDebug("Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message => {
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                };
            });
        }

        public static void Stop() {
            _server?.Dispose();
        }
    }
}
