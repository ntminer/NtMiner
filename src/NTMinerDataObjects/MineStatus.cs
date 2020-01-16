using System.ComponentModel;

namespace NTMiner {
    public enum MineStatus {
        [Description("全部")]
        All = 0,
        [Description("挖矿中")]
        Mining = 1,
        [Description("未挖矿")]
        UnMining = 2
    }
}
