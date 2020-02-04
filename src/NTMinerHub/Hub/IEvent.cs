namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        PathId TargetPathId { get; }
        DateTime BornOn { get; }
    }
}
