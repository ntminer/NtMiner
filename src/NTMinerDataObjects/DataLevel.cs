using System.ComponentModel;

namespace NTMiner {
    public enum DataLevel {
        [Description("未定义")]
        UnDefined,
        [Description("系统级")]
        Global,
        [Description("用户级")]
        Profile
    }
}
