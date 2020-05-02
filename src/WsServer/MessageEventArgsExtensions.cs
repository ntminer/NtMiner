using NTMiner.Ws;
using System;
using WebSocketSharp;

namespace NTMiner {
    public static class MessageEventArgsExtensions {
        public static T ToWsMessage<T>(this MessageEventArgs e) where T : WsMessage {
            if (e.IsBinary) {
                if (e.IsPing) {
                    throw new InvalidProgramException("Ping消息不应走到这一步");
                }
                return VirtualRoot.BinarySerializer.Deserialize<T>(e.RawData);
            }
            if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                return null;
            }
            return VirtualRoot.JsonSerializer.Deserialize<T>(e.Data);
        }
    }
}
