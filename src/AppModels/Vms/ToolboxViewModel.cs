using Microsoft.Win32;
using NTMiner.Core;
using System.Diagnostics;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ToolboxViewModel : ViewModelBase {
        public ICommand SwitchRadeonGpu { get; private set; }
        public ICommand AtikmdagPatcher { get; private set; }
        public ICommand NavigateToNvidiaDriverWin10 { get; private set; }
        public ICommand NavigateToNvidiaDriverWin7 { get; private set; }
        public ICommand NavigateToAmdDriver { get; private set; }
        public ICommand RegCmdHere { get; private set; }
        public ICommand BlockWAU { get; private set; }
        public ICommand Win10Optimize { get; private set; }
        public ICommand EnableWindowsRemoteDesktop { get; private set; }
        public ICommand WindowsAutoLogon { get; private set; }

        public ToolboxViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.SwitchRadeonGpu = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定运行吗？大概需要花费5到10秒钟时间看到结果", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new SwitchRadeonGpuCommand());
                }, icon: IconConst.IconConfirm);
            }, () => NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD && !MinerProfileViewModel.Instance.IsMining);
            this.AtikmdagPatcher = new DelegateCommand(() => {
                VirtualRoot.Execute(new AtikmdagPatcherCommand());
            }, () => NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD && !MinerProfileViewModel.Instance.IsMining);
            this.NavigateToNvidiaDriverWin10 = new DelegateCommand(() => {
                Process.Start("https://www.geforce.cn/drivers/results/137770");
            });
            this.NavigateToNvidiaDriverWin7 = new DelegateCommand(() => {
                Process.Start("https://www.geforce.cn/drivers/results/137752");
            });
            this.NavigateToAmdDriver = new DelegateCommand(() => {
                Process.Start("https://www.amd.com/zh-hans/support");
            });
            this.RegCmdHere = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定在windows右键上下文菜单中添加\"命令行\"菜单吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RegCmdHereCommand());
                }, icon: IconConst.IconConfirm);
            });
            this.BlockWAU = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定禁用Windows系统更新吗？禁用后可在Windows服务中找到Windows Update手动启用。", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new BlockWAUCommand());
                }, icon: IconConst.IconConfirm, helpUrl: "https://www.loserhub.cn/posts/details/91");
            });
            this.Win10Optimize = new DelegateCommand(() => {
                this.ShowDialog(message: $"确定面向挖矿优化windows吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new Win10OptimizeCommand());
                }, icon: IconConst.IconConfirm, helpUrl: "https://www.loserhub.cn/posts/details/83");
            });
            this.EnableWindowsRemoteDesktop = new DelegateCommand(() => {
                VirtualRoot.Execute(new EnableWindowsRemoteDesktopCommand());
            });
            this.WindowsAutoLogon = new DelegateCommand(() => {
                VirtualRoot.Execute(new EnableOrDisableWindowsAutoLoginCommand());
            });
        }

        public bool IsAutoAdminLogon {
            get { return Windows.OS.Instance.IsAutoAdminLogon; }
        }

        public string AutoAdminLogonMessage {
            get {
                if (IsAutoAdminLogon) {
                    return "开机自动登录已启用";
                }
                return "开机自动登录未启用";
            }
        }

        public bool IsRemoteDesktopEnabled {
            get {
                return NTMinerRoot.GetIsRemoteDesktopEnabled();
            }
        }

        public string RemoteDesktopMessage {
            get {
                if (IsRemoteDesktopEnabled) {
                    return "远程桌面已启用";
                }
                return "远程桌面已禁用";
            }
        }
    }
}
