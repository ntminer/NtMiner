using System;

namespace NTMiner.Core {
    public interface IKernelInput : IEntity<Guid> {
        string Name { get; }
        Guid DualCoinGroupId { get; }
        string Args { get; }
        bool IsSupportDualMine { get; }
        double DualWeightMin { get; }
        double DualWeightMax { get; }
        bool IsAutoDualWeight { get; }
        string DualWeightArg { get; }
        string DualFullArgs { get; }
    }
}
