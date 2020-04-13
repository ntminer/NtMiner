
using System;

namespace NTMiner.Hub {
    /// <summary>
    /// 消息是个承载信息的空间结构体，是一段树枝，树枝的枝杈相对位置和长短编码了消息收发方理解的信息。
    /// 消息分两种：命令 和 事件，不存在第三种。命令和事件的不同主要体现在时间上，命令是事情发生前的消息，事件是事情发生后的消息。
    /// </summary>
    public interface IMessage {
        Guid MessageId { get; }
    }
}
