namespace NTMiner {
    /// <summary>
    /// 最近一次停止挖矿的原因
    /// </summary>
    public enum StopMineReason {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown,
        /// <summary>
        /// 用户点击停止按钮
        /// </summary>
        LocalUserAction,
        /// <summary>
        /// 开始挖矿时的防御性编程，真正开始挖矿前调用一次停止挖矿操作
        /// </summary>
        InStartMine,
        /// <summary>
        /// CPU温度过高
        /// </summary>
        HighCpuTemperature,
        /// <summary>
        /// 用户通过群控远程停止挖矿
        /// </summary>
        RPCUserAction,
        /// <summary>
        /// 挖矿内核进程消失
        /// </summary>
        KernelProcessLost,
        /// <summary>
        /// 重启挖矿时，开始挖矿前调用一次停止挖矿操作
        /// </summary>
        RestartMine,
        /// <summary>
        /// 退出开源矿工
        /// </summary>
        ApplicationExit
    }
}
