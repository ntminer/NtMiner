using System;

namespace NTMiner.Core.Kernels {
    public interface IKernel : IEntity<Guid> {
        string Code { get; }
        string Version { get; }
        string FullName { get; }
        ulong PublishOn { get; }
        string Package { get; }
        string PackageHistory { get; }
        string Sha1 { get; }
        long Size { get; }
        int SortNumber { get; }
        Guid DualCoinGroupId { get; }
        PublishStatus PublishState { get; }
        string Args { get; }
        bool IsSupportDualMine { get; }
        double DualWeightMin { get; }
        double DualWeightMax { get; }
        bool IsAutoDualWeight { get; }
        string DualFullArgs { get; }
        string HelpArg { get; set; }
        string Notice { get; }

        string TotalSpeedPattern { get; }
        string TotalSharePattern { get; }
        string AcceptSharePattern { get; }
        string RejectSharePattern { get; }
        string RejectPercentPattern { get; }
        string GpuSpeedPattern { get; }

        string DualTotalSpeedPattern { get; }
        string DualTotalSharePattern { get; }
        string DualAcceptSharePattern { get; }
        string DualRejectSharePattern { get; }
        string DualRejectPercentPattern { get; }
        string DualGpuSpeedPattern { get; }
    }
}
