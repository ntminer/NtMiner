using System.ComponentModel;

namespace NTMiner.Core.Gpus {
    public enum GpuType {
        Empty,
        [Description("N卡")]
        NVIDIA,
        [Description("A卡")]
        AMD
    }
}
