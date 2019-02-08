namespace NTMiner.Bus {
    using NTMiner;
    using System;

    public interface IEvent : IMessage, IEntity<Guid> {
        DateTime Timestamp { get; }
    }
}
