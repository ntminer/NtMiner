namespace NTMiner.Bus {
    using System;

    public interface IEvent : IMessage, IEntity<Guid> {
        DateTime Timestamp { get; }
    }
}
