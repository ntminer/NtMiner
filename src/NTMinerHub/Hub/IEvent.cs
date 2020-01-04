namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        Guid RouteToPathId { get; }
        DateTime BornOn { get; }
    }
}
