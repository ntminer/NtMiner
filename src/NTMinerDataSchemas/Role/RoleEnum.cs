using System.ComponentModel;

namespace NTMiner.Role {
    public enum RoleEnum {
        [Description("超管")]
        Admin,
        [Description("普通用户")]
        User
    }
}
