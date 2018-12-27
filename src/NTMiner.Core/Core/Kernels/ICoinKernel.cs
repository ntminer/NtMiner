using NTMiner.Core.Gpus;
using System;

namespace NTMiner.Core.Kernels {
    public interface ICoinKernel : IEntity<Guid> {
        Guid CoinId { get; }
        Guid KernelId { get; }
        int SortNumber { get; }
        string Args { get; }
        string Description { get; }
        Guid DualCoinGroupId { get; }
        SupportedGpu SupportedGpu { get; }
    }
}
