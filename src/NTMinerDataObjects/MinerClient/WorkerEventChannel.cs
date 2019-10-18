namespace NTMiner.MinerClient {
    public enum WorkerEventChannel {
        Undefined = 0,
        /// <summary>
        /// 挖矿端本地事件
        /// </summary>
        MinerClient = 1,
        /// <summary>
        /// 服务端事件，比如服务端升级了内核版本时会通知群控客户端（如果挖矿端接收事件也通知挖矿端）
        /// ，此时群控客户端或挖矿端将收到的事件视为MinerServer频道的事件。
        /// </summary>
        MinerServer = 2,
        /// <summary>
        /// 群控客户端本地事件
        /// </summary>
        MinerStudio = 3,
        /// <summary>
        /// 基于内核输出和关键字提取的事件
        /// </summary>
        KernelOutput = 4
    }
}
