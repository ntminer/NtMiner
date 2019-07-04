using System;

namespace NTMiner.Core {
    public interface IKernelOutput : IEntity<Guid> {
        string Name { get; }
        bool PrependDateTime { get; }
        bool IsDualInSameLine { get; }

        string KernelRestartKeyword { get; }

        string TotalSpeedPattern { get; }
        string TotalSharePattern { get; }
        string AcceptSharePattern { get; }
        string AcceptOneShare { get; }
        string RejectSharePattern { get; }
        string RejectOneShare { get; }
        string RejectPercentPattern { get; }
        string GpuSpeedPattern { get; }

        string DualTotalSpeedPattern { get; }
        string DualTotalSharePattern { get; }
        string DualAcceptSharePattern { get; }
        string DualAcceptOneShare { get; }
        string DualRejectSharePattern { get; }
        string DualRejectOneShare { get; }
        string DualRejectPercentPattern { get; }
        string DualGpuSpeedPattern { get; }

        string PoolDelayPattern { get; }
        string DualPoolDelayPattern { get; }

        string SpeedUnit { get; }
        string DualSpeedUnit { get; }
        int GpuBaseIndex { get; }
    }
}
