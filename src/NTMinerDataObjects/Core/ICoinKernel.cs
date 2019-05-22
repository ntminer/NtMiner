using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICoinKernel : IEntity<Guid> {
        Guid CoinId { get; }
        Guid KernelId { get; }
        int SortNumber { get; }
        string Args { get; }
        Guid DualCoinGroupId { get; }
        SupportedGpu SupportedGpu { get; }
        string Notice { get; }
        List<EnvironmentVariable> EnvironmentVariables { get; }
        List<InputSegment> InputSegments { get; }
        List<Guid> FileWriterIds { get; }
    }
}
