using System.ComponentModel;

namespace NTMiner {
    public enum GpuType {
        [Description("没有矿卡或矿卡未驱动")]
        Empty,
        /// <summary>
        /// 名称已持久化，注意不要改名称
        /// </summary>
        [Description("N卡")]
        NVIDIA,
        /// <summary>
        /// 名称已持久化，注意不要改名称
        /// </summary>
        [Description("A卡")]
        AMD
    }
}
