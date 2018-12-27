using System.ComponentModel;

namespace NTMiner.Core.Gpus {
    public enum SupportedGpu {
        [Description("N卡")]
        NVIDIA,
        [Description("A卡")]
        AMD,
        [Description("N卡和A卡")]
        Both
    }
}
