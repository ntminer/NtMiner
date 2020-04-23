using System.ComponentModel;

namespace NTMiner.Core {
    public enum GpuType {
        [Description("请选择")]
        Empty = 0,
        /// <summary>
        /// 名称已持久化，注意不要改名称
        /// </summary>
        [Description("N卡")]
        NVIDIA = 1,
        /// <summary>
        /// 名称已持久化，注意不要改名称
        /// </summary>
        [Description("A卡")]
        AMD = 2
    }
}
