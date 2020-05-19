using System.ComponentModel;

namespace NTMiner.Core.MinerClient {
    public enum MinerClientActionType {
        [Description("A卡驱动签名，如果本机不是A卡将被忽略")]
        AtikmdagPatcher,
        [Description("开启A卡计算模式，如果本机不是A卡将被忽略")]
        SwitchRadeonGpuOn,
        [Description("关闭A卡计算模式，如果本机不是A卡将被忽略")]
        SwitchRadeonGpuOff,
        [Description("禁用Widnows系统更新")]
        BlockWAU
    }
}
