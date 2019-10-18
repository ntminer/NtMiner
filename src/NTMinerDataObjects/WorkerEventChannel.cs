using System.ComponentModel;

namespace NTMiner {
    /// <summary>
    /// 频道是平的，主题是分层的。开源矿工的挖矿事件没有主题需求。
    /// </summary>
    /// <remarks>持久层存的是枚举名</remarks>
    public enum WorkerEventChannel {
        [Description("不指定")]
        Unspecified,
        /// <summary>
        /// 这
        /// </summary>
        [Description("这")]
        This,
        /// <summary>
        /// 基于内核输出和关键字提取的事件
        /// </summary>
        [Description("内核")]
        Kernel,
        /// <summary>
        /// 服务端事件，比如服务端升级了内核版本时会通知群控客户端（如果挖矿端接收事件也通知挖矿端）
        /// ，此时群控客户端或挖矿端将收到的事件视为MinerServer频道的事件。
        /// </summary>
        [Description("服务器")]
        Server
    }
}
