using System.ComponentModel;

namespace NTMiner.MinerClient {
    /// <summary>
    /// 频道是平的，主题是分层的。开源矿工的挖矿消息没有主题需求。
    /// </summary>
    /// <remarks>持久层存的是枚举名</remarks>
    public enum LocalMessageChannel {
        [Description("全频道")]
        Unspecified,
        /// <summary>
        /// 我的频道
        /// </summary>
        [Description("我的频道")]
        This,
        /// <summary>
        /// 基于内核输出和关键字提取的事件
        /// </summary>
        [Description("内核频道")]
        Kernel
    }
}
