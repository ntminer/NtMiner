using System.ComponentModel;

namespace NTMiner {
    public enum GpuType {
        Empty,
        [Description("N卡")]
        NVIDIA,
        [Description("A卡")]
        AMD
    }
}
