using NTMiner.Core;
using System.Diagnostics;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ToolboxViewModel : ViewModelBase {
        public ICommand SwitchRadeonGpu { get; private set; }
        public ICommand AtikmdagPatcher { get; private set; }
        public ICommand NavigateToDriver { get; private set; }
        public ICommand RegCmdHere { get; private set; }
        public ICommand BlockWAU { get; private set; }
        public ICommand Win10Optimize { get; private set; }
        public ICommand EnableWindowsRemoteDesktop { get; private set; }
        public ICommand WindowsAutoLogon { get; private set; }

        public ICommand OpenDevmgmt { get; private set; }

        public ICommand OpenEventvwr { get; private set; }

        public ToolboxViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.SwitchRadeonGpu = new DelegateCommand(() => {
                if (MinerProfileViewModel.Instance.IsMining) {
                    VirtualRoot.Out.ShowInfo("请先停止挖矿");
                    return;
                }
                var config = new DialogWindowViewModel(
                    isConfirmNo: true,
                    btnNoToolTip: "注意：关闭计算模式挖矿算力会减半",
                    message: $"过程大概需要花费5到10秒钟", title: "确认", onYes: () => {
                        VirtualRoot.Execute(new SwitchRadeonGpuCommand(on: true));
                    }, onNo: () => {
                        VirtualRoot.Execute(new SwitchRadeonGpuCommand(on: false));
                        return true;
                    }, yesText: "开启计算模式", noText: "关闭计算模式");
                this.ShowSoftDialog(config);
            });
            this.AtikmdagPatcher = new DelegateCommand(() => {
                if (MinerProfileViewModel.Instance.IsMining) {
                    VirtualRoot.Out.ShowInfo("请先停止挖矿");
                    return;
                }
                VirtualRoot.Execute(new AtikmdagPatcherCommand());
            });
            this.NavigateToDriver = new DelegateCommand<SysDicItemViewModel>((item) => {
                if (item == null) {
                    return;
                }
                Process.Start(item.Value);
            });
            this.RegCmdHere = new DelegateCommand(() => {
                if (IsRegedCmdHere) {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定移除windows右键上下文菜单中的\"命令行\"菜单吗？", title: "确认", onYes: () => {
                        VirtualRoot.Execute(new UnRegCmdHereCommand());
                        OnPropertyChanged(nameof(IsRegedCmdHere));
                    }));
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定在windows右键上下文菜单中添加\"命令行\"菜单吗？", title: "确认", onYes: () => {
                        VirtualRoot.Execute(new RegCmdHereCommand());
                        OnPropertyChanged(nameof(IsRegedCmdHere));
                    }));
                }
            });
            this.BlockWAU = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定禁用Windows系统更新吗？禁用后可在Windows服务中找到Windows Update手动启用。", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new BlockWAUCommand());
                }, helpUrl: "https://www.cnblogs.com/ntminer/p/12155769.html"));
            });
            this.Win10Optimize = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定面向挖矿优化windows吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new Win10OptimizeCommand());
                }, helpUrl: "https://www.cnblogs.com/ntminer/p/12155773.html"));
            });
            this.EnableWindowsRemoteDesktop = new DelegateCommand(() => {
                VirtualRoot.Execute(new EnableWindowsRemoteDesktopCommand());
            });
            this.WindowsAutoLogon = new DelegateCommand(() => {
                VirtualRoot.Execute(new EnableOrDisableWindowsAutoLoginCommand());
            });
            this.OpenDevmgmt = new DelegateCommand(() => {
                Process.Start("devmgmt.msc");
            });
            this.OpenEventvwr = new DelegateCommand(() => {
                Process.Start("eventvwr.msc", "/c:Application");
            });
        }

        public bool IsRegedCmdHere {
            get {
                return Windows.Cmd.IsRegedCmdHere();
            }
        }

        public bool IsReturnEthDevFee {
            get {
                return false;
            }
        }

        public SysDicItemViewModel NvidiaDriverWin10 {
            get {
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "NvidiaDriverWin10", out ISysDicItem item)) {
                    if (AppContext.Instance.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel NvidiaDriverWin7 {
            get {
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "NvidiaDriverWin7", out ISysDicItem item)) {
                    if (AppContext.Instance.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel NvidiaDriverMore {
            get {
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "NvidiaDriverMore", out ISysDicItem item)) {
                    if (AppContext.Instance.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel AmdDriverMore {
            get {
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "AmdDriverMore", out ISysDicItem item)) {
                    if (AppContext.Instance.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
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
                return NTMinerRegistry.GetIsRemoteDesktopEnabled();
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
