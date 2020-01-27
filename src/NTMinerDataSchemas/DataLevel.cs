using System.ComponentModel;

namespace NTMiner {
    // 开源矿工挖矿端的数据库记录分两层，global层是开源矿工官方管理的全局数据，profile层是矿工管理的自定义数据。
    // Profile级数据：对Global级数据和程序静态数据勾勾选选时生成的数据。
    public enum DataLevel {
        [Description("未定义")]
        UnDefined,
        [Description("系统级")]
        Global = 1,
        [Description("用户级")]
        Profile = 2
    }
}
