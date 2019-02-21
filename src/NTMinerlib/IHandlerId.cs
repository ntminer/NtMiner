using System;

namespace NTMiner {
    public interface IHandlerId : IEntity<Guid> {
        Guid Id { get; }
        Type MessageType { get; }
        Type Location { get; }
        string HandlerPath { get; }
        LogEnum LogType { get; }
        string Description { get; }
    }
}
