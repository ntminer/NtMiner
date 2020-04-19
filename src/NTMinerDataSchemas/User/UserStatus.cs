using System.ComponentModel;

namespace NTMiner.User {
    public enum UserStatus {
        [Description("全部")]
        All = 0,
        [Description("启用中")]
        IsEnabled = 1,
        [Description("禁用中")]
        IsDisabled = 2
    }
}
