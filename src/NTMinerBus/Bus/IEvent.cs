namespace NTMiner.Bus {
    using System;

    public interface IEvent : IMessage {
        DateTime Timestamp { get; }
    }
}
