using System;

namespace NTMiner.Bus {
    
    public interface IMessagePathId {
        Guid PathId { get; }
        int ViaLimit { get; }
        Type MessageType { get; }
        bool IsEnabled { get; set; }
        Type Location { get; }
        string Path { get; }
        LogEnum LogType { get; }
        string Description { get; }
    }
}
