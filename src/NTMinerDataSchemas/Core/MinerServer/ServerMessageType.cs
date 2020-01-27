using System.ComponentModel;

namespace NTMiner.Core.MinerServer {
    /// <remarks>持久层存的是枚举名</remarks>
    public enum ServerMessageType {
        [Description("消息")]
        Info = 0,
        [Description("新版本")]
        NewVersion = 1
    }
}
