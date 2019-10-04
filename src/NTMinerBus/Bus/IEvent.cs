namespace NTMiner.Bus {
    using System;

    /// <summary>
    /// 事件
    /// </summary>
    public interface IEvent : IMessage {
        DateTime Timestamp { get; }
    }
}
