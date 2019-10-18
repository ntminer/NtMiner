using System.ComponentModel;

namespace NTMiner {
    /// <summary>
    /// 频道是平的，主题是分层的。开源矿工的挖矿事件没有主题需求。
    /// </summary>
    /// <remarks>持久层存的是枚举名</remarks>
    public enum WorkerEventChannel {
        [Description("未定义")]
        Undefined = 0,
        /// <summary>
        /// 挖矿端本地事件或来自挖矿端的事件
        /// </summary>
        [Description("挖矿端")]
        MinerClient = 1,
        /// <summary>
        /// 服务端事件，比如服务端升级了内核版本时会通知群控客户端（如果挖矿端接收事件也通知挖矿端）
        /// ，此时群控客户端或挖矿端将收到的事件视为MinerServer频道的事件。
        /// </summary>
        [Description("服务端")]
        MinerServer = 2,
        /// <summary>
        /// 群控客户端本地事件或来自群控客户端的事件
        /// </summary>
        [Description("群控端")]
        MinerStudio = 3,
        /// <summary>
        /// 挖矿端守护进程本地事件或来自挖矿端守护进程的事件
        /// </summary>
        [Description("守护")]
        MinerDaemon = 4,
        /// <summary>
        /// 基于内核输出和关键字提取的事件
        /// </summary>
        [Description("内核输出")]
        KernelOutput = 5,
        /// <summary>
        /// 远程调用
        /// </summary>
        [Description("远程调用")]
        RPC,
    }
}
