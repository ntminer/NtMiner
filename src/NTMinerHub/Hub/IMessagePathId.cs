using System;

namespace NTMiner.Hub {
    
    public interface IMessagePathId {
        PathId PathId { get; }
        PathPriority Priority { get; }
        DateTime CreatedOn { get; }
        /// <summary>
        /// 表示该条消息路径允许消息通过的次数
        /// </summary>
        int ViaTimesLimit { get; }
        Type MessageType { get; }
        MessageTypeAttribute MessageTypeAttribute { get; }
        bool IsEnabled { get; set; }
        Type Location { get; }
        string Path { get; }
        LogEnum LogType { get; }
        string Description { get; }
    }
}
