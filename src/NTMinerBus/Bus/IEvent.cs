namespace NTMiner.Bus {
    using System;

    public interface IEvent : IMessage {
        Guid BornPathId { get; }
        DateTime Timestamp { get; }
    }
}
