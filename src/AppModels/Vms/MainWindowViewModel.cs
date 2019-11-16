using NTMiner.Core;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private readonly StateBarViewModel _stateBarVm = new StateBarViewModel();
        private SolidColorBrush _daemonStateBrush;

        public ICommand UseThisPcName { get; private set; }
        public ICommand CloseMainWindow { get; private set; }

        public MainWindowViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            RefreshDaemonStateBrush();
            this.CloseMainWindow = new DelegateCommand(() => {
                VirtualRoot.Execute(new CloseMainWindowCommand("已切换为无界面模式运行"));
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.ThisPcName;
                this.ShowDialog(new DialogWindowViewModel(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }));
            });
        }

        public bool IsTestHost {
            get {
                return !string.IsNullOrEmpty(Hosts.GetIp(NTKeyword.ServerHost, out long _));
            }
            set {
                if (value) {
                    Hosts.SetHost(NTKeyword.ServerHost, "127.0.0.1");
                }
                else {
                    Hosts.SetHost(NTKeyword.ServerHost, string.Empty);
                }
                OnPropertyChanged(nameof(IsTestHost));
            }
        }

        public string BrandTitle {
            get {
                if (NTMinerRoot.KernelBrandId == Guid.Empty && NTMinerRoot.PoolBrandId == Guid.Empty) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTMinerRoot.KernelBrandId, out ISysDicItem dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                else if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTMinerRoot.PoolBrandId, out dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                return string.Empty;
            }
        }

        public StateBarViewModel StateBarVm {
            get => _stateBarVm;
        }

        public bool IsUseDevConsole {
            get { return NTMinerRoot.IsUseDevConsole; }
            set {
                NTMinerRoot.IsUseDevConsole = value;
                OnPropertyChanged(nameof(IsUseDevConsole));
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }

        public SolidColorBrush DaemonStateBrush {
            get => _daemonStateBrush;
            set {
                if (_daemonStateBrush != value) {
                    _daemonStateBrush = value;
                    OnPropertyChanged(nameof(DaemonStateBrush));
                }
            }
        }

        public void RefreshDaemonStateBrush() {
            if (NTMinerRoot.Instance.CreatedOn.AddSeconds(10) > DateTime.Now) {
                // 如果刚刚启动10秒钟内视为白色正常状态
                DaemonStateBrush = WpfUtil.WhiteBrush;
            }
            else if (NTMinerRegistry.GetDaemonActiveOn().AddSeconds(20) >= DateTime.Now) {
                DaemonStateBrush = WpfUtil.WhiteBrush;
            }
            else {
                DaemonStateBrush = WpfUtil.RedBrush;
            }
        }
    }
}
