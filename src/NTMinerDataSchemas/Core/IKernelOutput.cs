using System;

namespace NTMiner.Core {
    // 注意：任何属性都不要改名，因为已进入server.json
    public interface IKernelOutput : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        bool IsDualInSameLine { get; }

        string KernelRestartKeyword { get; }

        string TotalSpeedPattern { get; }
        string TotalSharePattern { get; }
        string AcceptSharePattern { get; }
        string RejectSharePattern { get; }
        string RejectPercentPattern { get; }
        string GpuSpeedPattern { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string FoundOneShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string AcceptOneShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string RejectOneShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string GpuGotOneIncorrectShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string GpuAcceptShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string GpuRejectShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string GpuIncorrectShare { get; }

        string DualTotalSpeedPattern { get; }
        string DualTotalSharePattern { get; }
        string DualAcceptSharePattern { get; }
        string DualRejectSharePattern { get; }
        string DualRejectPercentPattern { get; }
        string DualGpuSpeedPattern { get; }

        string PoolDelayPattern { get; }
        string DualPoolDelayPattern { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string DualAcceptOneShare { get; }
        /// <summary>
        /// 正则
        /// </summary>
        string DualRejectOneShare { get; }

        string SpeedUnit { get; }
        string DualSpeedUnit { get; }
        int GpuBaseIndex { get; }
        /// <summary>
        /// 非全选显卡时映射显卡编号
        /// </summary>
        bool IsMapGpuIndex { get; }
    }
}
