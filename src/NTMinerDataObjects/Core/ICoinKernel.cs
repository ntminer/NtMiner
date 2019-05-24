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
        /// <summary>
        /// 表示是否支持备用矿池1
        /// </summary>
        bool IsSupportPool1 { get; }
        string Notice { get; }
        List<EnvironmentVariable> EnvironmentVariables { get; }
        List<InputSegment> InputSegments { get; }
        List<Guid> FileWriterIds { get; }
    }
}
