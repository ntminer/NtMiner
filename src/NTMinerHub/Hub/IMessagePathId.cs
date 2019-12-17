using System;

namespace NTMiner.Hub {
    
    public interface IMessagePathId {
        Guid PathId { get; }
        DateTime CreatedOn { get; }
        int ViaLimit { get; }
        Type MessageType { get; }
        bool IsEnabled { get; set; }
        Type Location { get; }
        string Path { get; }
        LogEnum LogType { get; }
        string Description { get; }
    }
}
