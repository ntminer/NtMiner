using NTMiner.Ws;
using System;
using WebSocketSharp;

namespace NTMiner {
    public static class MessageEventArgsExtensions {
        public static T ToWsMessage<T>(this MessageEventArgs e) where T : WsMessage {
            // IsPing、IsBinary、IsText三者是互斥的，经查源码实际上是判断一个Opcode枚举类型的值
            if (e.IsPing) {
                throw new InvalidProgramException("Ping消息不应走到这一步");
            }
            if (e.IsBinary) {
                return VirtualRoot.BinarySerializer.Deserialize<T>(e.RawData);
            }
            else {
                if (string.IsNullOrEmpty(e.Data) || e.Data[0] != '{' || e.Data[e.Data.Length - 1] != '}') {
                    return null;
                }
                return VirtualRoot.JsonSerializer.Deserialize<T>(e.Data);
            }
        }
    }
}
