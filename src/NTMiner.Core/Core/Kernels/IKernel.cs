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
        PublishStatus PublishState { get; }

        Guid DualCoinGroupId { get; }
        string Args { get; }
        bool IsSupportDualMine { get; }
        double DualWeightMin { get; }
        double DualWeightMax { get; }
        bool IsAutoDualWeight { get; }
        string DualWeightArg { get; }
        string DualFullArgs { get; }

        string HelpArg { get; set; }
        string Notice { get; }
        Guid KernelInputId { get; }
        Guid KernelOutputId { get; }
    }
}
