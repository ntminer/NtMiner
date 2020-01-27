using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICoinKernel : IEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }
        Guid KernelId { get; }
        string Args { get; }
        string DualFullArgs { get; }
        Guid DualCoinGroupId { get; }
        SupportedGpu SupportedGpu { get; }
        /// <summary>
        /// 表示是否支持备用矿池1
        /// </summary>
        bool IsSupportPool1 { get; }
        string Notice { get; }
        List<EnvironmentVariable> EnvironmentVariables { get; }
        /// <summary>
        /// Segment是里面没有变量的分割片段
        /// </summary>
        List<InputSegment> InputSegments { get; }
        List<Guid> FileWriterIds { get; }
        /// <summary>
        /// Fragment是里面有变量的片段
        /// </summary>
        List<Guid> FragmentWriterIds { get; }
        bool IsHot { get; }
        bool IsRecommend { get; }
    }
}
