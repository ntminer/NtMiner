using System;

namespace NTMiner.Core {
    public interface IKernelInput : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        string Args { get; }
        bool IsSupportDualMine { get; }
        double DualWeightMin { get; }
        double DualWeightMax { get; }
        bool IsAutoDualWeight { get; }
        string DualWeightArg { get; }
        bool IsDeviceAllNotEqualsNone { get; }
        string DevicesArg { get; }
        int DeviceBaseIndex { get; }
        string DevicesSeparator { get; }
        string NDevicePrefix { get; }
        string NDevicePostfix { get; }
        string ADevicePrefix { get; }
        string ADevicePostfix { get; }
    }
}
