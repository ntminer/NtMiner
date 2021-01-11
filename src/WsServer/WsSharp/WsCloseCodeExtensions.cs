using WebSocketSharp;

namespace NTMiner.WsSharp {
    public static class WsCloseCodeExtensions {
        public static CloseStatusCode ToCloseStatusCode(this WsCloseCode code) {
            switch (code) {
                case WsCloseCode.Normal:
                    return CloseStatusCode.Normal;
                case WsCloseCode.Away:
                    return CloseStatusCode.Away;
                case WsCloseCode.ProtocolError:
                    return CloseStatusCode.ProtocolError;
                case WsCloseCode.Abnormal:
                    return CloseStatusCode.Abnormal;
                default:
                    return CloseStatusCode.Normal;
            }
        }
    }
}
