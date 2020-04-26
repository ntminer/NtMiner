using NTMiner.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ToolboxViewModel : ViewModelBase {
        private bool _isWinCmdLoading = false;
        private double _winCmdLodingIconAngle;

        public ICommand SwitchRadeonGpu { get; private set; }
        public ICommand AtikmdagPatcher { get; private set; }
        public ICommand RegCmdHere { get; private set; }
        public ICommand BlockWAU { get; private set; }
        public ICommand Win10Optimize { get; private set; }
        public ICommand EnableRemoteDesktop { get; private set; }
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
                    }, btnYesText: "开启计算模式", btnNoText: "关闭计算模式");
                this.ShowSoftDialog(config);
            });
            this.AtikmdagPatcher = new DelegateCommand(() => {
                if (MinerProfileViewModel.Instance.IsMining) {
                    VirtualRoot.Out.ShowInfo("请先停止挖矿");
                    return;
                }
                VirtualRoot.Execute(new AtikmdagPatcherCommand());
            });
            this.RegCmdHere = new DelegateCommand(() => {
                if (IsRegedCmdHere) {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定移除windows右键上下文菜单中的\"命令行\"菜单吗？", title: "确认", onYes: () => {
                        Task.Factory.StartNew(() => {
                            VirtualRoot.Execute(new UnRegCmdHereCommand());
                            OnPropertyChanged(nameof(IsRegedCmdHere));
                        });
                    }, btnYesText: "移除"));
                }
                else {
                    this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定在windows右键上下文菜单中添加\"命令行\"菜单吗？", title: "确认", onYes: () => {
                        Task.Factory.StartNew(() => {
                            this.IsWinCmdLoading = true;
                            VirtualRoot.Execute(new RegCmdHereCommand());
                        });
                    }, btnYesText: "添加"));
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
            this.EnableRemoteDesktop = new DelegateCommand(() => {
                VirtualRoot.Execute(new EnableRemoteDesktopCommand());
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

        public bool IsWinCmdLoading {
            get => _isWinCmdLoading;
            set {
                if (_isWinCmdLoading != value) {
                    _isWinCmdLoading = value;
                    OnPropertyChanged(nameof(IsWinCmdLoading));
                    if (value) {
                        VirtualRoot.SetInterval(per: TimeSpan.FromMilliseconds(100), perCallback: () => {
                            this.WinCmdLodingIconAngle += 30;
                        }, stopCallback: () => {
                            this.IsWinCmdLoading = false;
                            OnPropertyChanged(nameof(IsRegedCmdHere));
                        }, timeout: TimeSpan.FromSeconds(6), requestStop: () => {
                            return IsRegedCmdHere;
                        });
                    }
                }
            }
        }

        public double WinCmdLodingIconAngle {
            get => _winCmdLodingIconAngle;
            set {
                _winCmdLodingIconAngle = value;
                OnPropertyChanged(nameof(WinCmdLodingIconAngle));
            }
        }

        public bool IsReturnEthDevFee {
            get {
                return false;
            }
        }

        public SysDicItemViewModel NvidiaDriverWin10 {
            get {
                if (NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "NvidiaDriverWin10", out ISysDicItem item)) {
                    if (AppRoot.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel NvidiaDriverWin7 {
            get {
                if (NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "NvidiaDriverWin7", out ISysDicItem item)) {
                    if (AppRoot.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel NvidiaDriverMore {
            get {
                if (NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "NvidiaDriverMore", out ISysDicItem item)) {
                    if (AppRoot.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel AmdDriverMore {
            get {
                if (NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "AmdDriverMore", out ISysDicItem item)) {
                    if (AppRoot.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel VisualCpp0 {
            get {
                if (NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "VisualCpp0", out ISysDicItem item)) {
                    if (AppRoot.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
                        return vm;
                    }
                }
                return null;
            }
        }

        public SysDicItemViewModel VisualCpp1 {
            get {
                if (NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "VisualCpp1", out ISysDicItem item)) {
                    if (AppRoot.SysDicItemVms.TryGetValue(item.GetId(), out SysDicItemViewModel vm)) {
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
                    return "已开启";
                }
                return "未开启";
            }
        }

        public bool IsRemoteDesktopEnabled {
            get {
                return NTMinerRegistry.GetIsRdpEnabled();
            }
        }

        public string RemoteDesktopMessage {
            get {
                if (IsRemoteDesktopEnabled) {
                    return "已开启";
                }
                return "未开启";
            }
        }
    }
}
