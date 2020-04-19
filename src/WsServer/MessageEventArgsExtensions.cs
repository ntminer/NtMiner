using NTMiner.Ws;
using WebSocketSharp;

namespace NTMiner {
    public static class MessageEventArgsExtensions {
        public static T ToWsMessage<T>(this MessageEventArgs e) where T : WsMessage {
            if (!e.IsText) {
                return null;
            }
            if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                return null;
            }
            return VirtualRoot.JsonSerializer.Deserialize<T>(e.Data);
        }
    }
}
