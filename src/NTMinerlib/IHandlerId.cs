using System;

namespace NTMiner {
    public interface IHandlerId {
        Type MessageType { get; }
        Type Location { get; }
        string HandlerPath { get; }
        LogEnum LogType { get; }
        string Description { get; }
    }
}
