using System.ComponentModel;

namespace NTMiner {
    /// <remarks>持久层存的是枚举名</remarks>
    public enum WorkerEventType {
        [Description("未定义")]
        Undefined,
        [Description("消息")]
        Info,
        [Description("警告")]
        Warn,
        [Description("错误")]
        Error
    }
}
