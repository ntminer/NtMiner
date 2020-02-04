using System.ComponentModel;

namespace NTMiner.Core.MinerClient {
    /// <remarks>持久层存的是枚举名</remarks>
    public enum LocalMessageType {
        [Description("消息")]
        Info = 0,
        [Description("警告")]
        Warn = 1,
        [Description("错误")]
        Error = 2
    }
}
