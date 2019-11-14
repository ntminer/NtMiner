using System.ComponentModel;

namespace NTMiner.Core.Kernels {
    public enum InstallStatus {
        [Description("未安装")]
        Uninstalled,
        [Description("已安装")]
        Installed
    }
}
