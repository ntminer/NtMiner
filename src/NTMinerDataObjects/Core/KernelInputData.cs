using System;

namespace NTMiner.Core {
    public class KernelInputData : IKernelInput, IDbEntity<Guid> {
        public KernelInputData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
        public Guid DualCoinGroupId { get; set; }
        public string Args { get; set; }
        public bool IsSupportDualMine { get; set; }
        public double DualWeightMin { get; set; }
        public double DualWeightMax { get; set; }
        public bool IsAutoDualWeight { get; set; }
        public string DualWeightArg { get; set; }
        public string DualFullArgs { get; set; }
        public bool IsDeviceAllNotEqualsNone { get; set; }
        public string DevicesArg { get; set; }
        public int DeviceBaseIndex { get; set; }
        public string DevicesSeparator { get; set; }
        public string NDevicePrefix { get; set; }
        public string NDevicePostfix { get; set; }
        public string ADevicePrefix { get; set; }
        public string ADevicePostfix { get; set; }
    }
}
