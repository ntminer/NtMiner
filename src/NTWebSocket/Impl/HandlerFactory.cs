using System;
using NTWebSocket.Impl.Handlers;

namespace NTWebSocket.Impl {
    public static class HandlerFactory {
        public static IHandler BuildHandler(
            WebSocketHttpRequest request, 
            Action<string> onMessage, 
            Action onClose, 
            Action<byte[]> onBinary, 
            Action<byte[]> onPing, 
            Action<byte[]> onPong) {

            var version = GetVersion(request);

            switch (version) {
                case "76":
                    return Draft76Handler.Create(request, onMessage);
                case "7":
                case "8":
                case "13":
                    return Hybi13Handler.Create(request, onMessage, onClose, onBinary, onPing, onPong);
                case "policy-file-request":
                    return FlashSocketPolicyRequestHandler.Create(request);
            }

            NTMiner.Write.DevError("UnsupportedDataType:" + version);
            return null;
        }

        public static string GetVersion(WebSocketHttpRequest request) {
            if (request.Headers.TryGetValue("Sec-WebSocket-Version", out string version)) {
                return version;
            }

            if (request.Headers.TryGetValue("Sec-WebSocket-Draft", out version)) {
                return version;
            }

            // safariä¯ÀÀÆ÷
            if (request.Headers.ContainsKey("Sec-WebSocket-Key1")) {
                return "76";
            }

            if ((request.Body != null) && request.Body.ToLower().Contains("policy-file-request")) {
                return "policy-file-request";
            }

            return "75";
        }
    }
}

