using System.ComponentModel;

namespace NTMiner {
    public enum GpuType {
        [Description("未指定")]
        Empty,
        [Description("N卡")]
        NVIDIA,
        [Description("A卡")]
        AMD
    }
}
