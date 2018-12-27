using System.ComponentModel;

namespace NTMiner.ServiceContracts.DataObjects {
    public enum MineStatus {
        [Description("全部")]
        All,
        [Description("挖矿中")]
        Mining,
        [Description("未挖矿")]
        UnMining
    }
}
