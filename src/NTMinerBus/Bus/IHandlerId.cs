using System;

namespace NTMiner.Bus {
    /// <summary>
    /// 处理器标识
    /// </summary>
    public interface IHandlerId {
        Type MessageType { get; }
        bool IsEnabled { get; set; }
        Type Location { get; }
        string HandlerPath { get; }
        LogEnum LogType { get; }
        string Description { get; }
    }
}
