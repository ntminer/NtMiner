namespace NTMiner.Bus {
    using System;

    public interface IEvent : IMessage {
        Guid PathId { get; }
        DateTime Timestamp { get; }
    }
}
