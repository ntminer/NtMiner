using System;

namespace NTMiner.Hub {
    
    public interface IMessagePathId {
        /// <summary>
        /// 该消息路径的标识，是个Guid。
        /// </summary>
        PathId PathId { get; }
        /// <summary>
        /// 消息路径优先级，相同消息类型的路径可能有多条，该优先级定义了路径的行走顺序。
        /// </summary>
        PathPriority Priority { get; }
        DateTime CreatedOn { get; }
        /// <summary>
        /// 该路径可以通行消息的次数，如果限定了次数则达到通行次数后路径消失，-1表示不限制次数。
        /// </summary>
        int ViaTimesLimit { get; }
        Type MessageType { get; }
        MessageTypeAttribute MessageTypeAttribute { get; }
        bool IsEnabled { get; set; }
        /// <summary>
        /// 该消息路径所处的位置（对程序员有意义的编码时源码位置，对运行时无意义）。
        /// </summary>
        string Location { get; }
        string PathName { get; }
        /// <summary>
        /// 日志类型，表示消息每通过一次是否记录日志以及记录什么日志，对程序员编程时有意义对运行时无意义。
        /// </summary>
        LogEnum LogType { get; }
        /// <summary>
        /// 描述该消息类型，对程序员编程时有意义，对运行时无意义。
        /// </summary>
        string Description { get; }
    }
}
