using System.ComponentModel;

namespace NTMiner.MinerClient {
    /// <remarks>持久层存的是枚举名</remarks>
    public enum LocalMessageType {
        [Description("消息")]
        Info,
        [Description("警告")]
        Warn,
        [Description("错误")]
        Error
    }
}
