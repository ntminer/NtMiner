using System;

namespace NTMiner.Core {
    public interface IKernelOutput : IEntity<Guid> {
        string Name { get; }
        bool PrependDateTime { get; }
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
    }
}
