using System.ComponentModel;

namespace NTMiner {
    /// <summary>
    /// 最近一次停止挖矿的原因
    /// </summary>
    public enum StopMineReason {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown,
        /// <summary>
        /// 用户点击停止按钮
        /// </summary>
        [Description("用户点击停止按钮")]
        LocalUserAction,
        /// <summary>
        /// 开始挖矿时前保障，真正开始挖矿前做一次停止挖矿操作
        /// </summary>
        [Description("开始挖矿时前保障，真正开始挖矿前做一次停止挖矿操作")]
        InStartMine,
        /// <summary>
        /// CPU温度过高
        /// </summary>
        [Description("CPU温度过高")]
        HighCpuTemperature,
        /// <summary>
        /// 用户通过群控远程停止挖矿
        /// </summary>
        [Description("用户通过群控远程停止挖矿")]
        RPCUserAction,
        /// <summary>
        /// 挖矿内核进程消失
        /// </summary>
        [Description("挖矿内核进程消失")]
        KernelProcessLost,
        /// <summary>
        /// 退出开源矿工
        /// </summary>
        [Description("退出开源矿工")]
        ApplicationExit,
        /// <summary>
        /// 重启电脑
        /// </summary>
        [Description("重启电脑")]
        RestartComputer
    }
}
