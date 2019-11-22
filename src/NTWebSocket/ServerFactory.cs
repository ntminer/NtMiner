using NTWebSocket.Impl;
using System.Net;

namespace NTWebSocket {
    public class ServerFactory {
        public static IWebSocketServer Create(SchemeType scheme, IPAddress ip, int port, bool supportDualStack = true) {
            return new WebSocketServer(scheme, ip, port, supportDualStack);
        }
    }
}
