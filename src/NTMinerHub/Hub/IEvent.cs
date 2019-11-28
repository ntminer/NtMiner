namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        Guid BornPathId { get; }
        DateTime BornOn { get; }
    }
}
