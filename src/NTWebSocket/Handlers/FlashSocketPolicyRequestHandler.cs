using System.Text;

namespace NTWebSocket.Handlers {
    public class FlashSocketPolicyRequestHandler {
        public static string PolicyResponse =
"<?xml version=\"1.0\"?>\n" +
"<cross-domain-policy>\n" +
"   <allow-access-from domain=\"*\" to-ports=\"*\"/>\n" +
"   <site-control permitted-cross-domain-policies=\"all\"/>\n" +
"</cross-domain-policy>\n" +
"\0";

        public static IHandler Create(WebSocketHttpRequest request) {
            return new ComposableHandler(handshake: sub => Handshake(request, sub));
        }

        public static byte[] Handshake(WebSocketHttpRequest request, string subProtocol) {
            NTWebSocketLog.Debug("Building Flash Socket Policy Response");
            return Encoding.UTF8.GetBytes(PolicyResponse);
        }
    }
}

