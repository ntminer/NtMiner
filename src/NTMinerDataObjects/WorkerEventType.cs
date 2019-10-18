using System.ComponentModel;

namespace NTMiner {
    /// <remarks>持久层存的是枚举名</remarks>
    public enum WorkerEventType {
        [Description("未定义")]
        Undefined = 0,
        [Description("消息")]
        Info = 1,
        [Description("警告")]
        Warn = 2,
        [Description("错误")]
        Error = 3
    }
}
