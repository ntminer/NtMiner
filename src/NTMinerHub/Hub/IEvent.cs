namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        PathId RouteToPathId { get; }
        DateTime BornOn { get; }
    }
}
