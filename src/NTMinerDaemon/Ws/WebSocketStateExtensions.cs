using WebSocketSharp;

namespace NTMiner.Ws {
    public static class WebSocketStateExtensions {
        public static WsClientStatus ToWsClientStatus(this WebSocketState readyState) {
            switch (readyState) {
                case WebSocketState.Connecting:
                    return WsClientStatus.Connecting;
                case WebSocketState.Open:
                    return WsClientStatus.Open;
                case WebSocketState.Closing:
                    return WsClientStatus.Closing;
                case WebSocketState.Closed:
                    return WsClientStatus.Closed;
                default:
                    return WsClientStatus.Closed;
            }
        }
    }
}
