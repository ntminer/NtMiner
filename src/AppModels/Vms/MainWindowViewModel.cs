using NTMiner.Core;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private string _serverJsonVersion;
        private readonly StateBarViewModel _stateBarVm = new StateBarViewModel();
        private SolidColorBrush _daemonStateBrush;

        public ICommand CustomTheme { get; private set; }
        public ICommand UseThisPcName { get; private set; }
        public ICommand CloseMainWindow { get; private set; }

        public MainWindowViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            RefreshDaemonStateBrush();
            this.CloseMainWindow = new DelegateCommand(() => {
                VirtualRoot.Execute(new CloseMainWindowCommand("已切换为无界面模式运行"));
            });
            this.CustomTheme = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowLogColorCommand());
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.ThisPcName;
                this.ShowDialog(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }, icon: IconConst.IconConfirm);
            });
            if (DevMode.IsDevMode) {
                _serverJsonVersion = NTMinerRoot.Instance.GetServerJsonVersion();
            }
        }

        public string BrandTitle {
            get {
                if (NTMinerRoot.KernelBrandId == Guid.Empty && NTMinerRoot.PoolBrandId == Guid.Empty) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.SysDicItemSet.TryGetDicItem(NTMinerRoot.KernelBrandId, out ISysDicItem dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                else if (NTMinerRoot.Instance.SysDicItemSet.TryGetDicItem(NTMinerRoot.PoolBrandId, out dicItem)) {
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

        public string ServerJsonVersion {
            get => _serverJsonVersion;
            set {
                if (_serverJsonVersion != value) {
                    _serverJsonVersion = value;
                    OnPropertyChanged(nameof(ServerJsonVersion));
                }
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
                DaemonStateBrush = Wpf.Util.WhiteBrush;
            }
            else if (NTMinerRegistry.GetDaemonActiveOn().AddSeconds(20) >= DateTime.Now) {
                DaemonStateBrush = Wpf.Util.WhiteBrush;
            }
            else {
                DaemonStateBrush = Wpf.Util.RedBrush;
            }
        }
    }
}
