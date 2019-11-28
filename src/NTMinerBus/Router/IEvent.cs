namespace NTMiner.Router {
    using System;

    public interface IEvent : IMessage {
        Guid BornPathId { get; }
        DateTime BornOn { get; }
    }
}
