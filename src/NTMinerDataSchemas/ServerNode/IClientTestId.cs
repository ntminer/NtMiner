using System;

namespace NTMiner.ServerNode {
    public interface IClientTestId {
        Guid MinerClientTestId { get; }
        Guid StudioClientTestId { get; }
    }
}
