using System.ComponentModel;

namespace NTMiner.MinerServer {
    /// <remarks>持久层存的是枚举名</remarks>
    public enum ServerMessageType {
        [Description("消息")]
        Info,
        [Description("新版本")]
        NewVersion
    }
}
