using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICoinKernel : IEntity<Guid> {
        Guid CoinId { get; }
        Guid KernelId { get; }
        int SortNumber { get; set; }
        string Args { get; }
        string Description { get; }
        Guid DualCoinGroupId { get; }
        SupportedGpu SupportedGpu { get; }
        List<EnvironmentVariable> EnvironmentVariables { get; }
    }
}
