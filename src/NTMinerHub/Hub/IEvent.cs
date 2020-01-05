namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        RouteToPathId RouteToPathId { get; }
        DateTime BornOn { get; }
    }
}
