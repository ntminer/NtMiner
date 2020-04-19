using System.ComponentModel;

namespace NTMiner.Ws {
    public enum WsClientStatus : ushort {
        [Description("连接中…")]
        Connecting = 0,
        [Description("连接服务器成功")]
        Open = 1,
        [Description("关闭中…")]
        Closing = 2,
        [Description("连接已断开")]
        Closed = 3
    }
}
