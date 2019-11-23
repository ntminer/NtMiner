using NTWebSocket.Impl;

namespace NTWebSocket {
    public class ServerFactory {
        public static IWebSocketServer Create(ServerConfig config) {
            return new WebSocketServer(config);
        }
    }
}
