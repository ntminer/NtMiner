using System;

namespace NTMiner.MinerClient {
    public interface IMinerEventType : ILevelEntity<Guid> {
        string Name { get; }
    }
}
