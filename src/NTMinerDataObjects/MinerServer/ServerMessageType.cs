using System.ComponentModel;

namespace NTMiner.MinerServer {
    public enum ServerMessageType {
        [Description("消息")]
        Info,
        [Description("新版本")]
        NewVersion
    }
}
