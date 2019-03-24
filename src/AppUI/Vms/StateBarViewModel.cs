using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class StateBarViewModel : ViewModelBase {
        public static readonly StateBarViewModel Current = new StateBarViewModel();

        private TimeSpan _mineTimeSpan = TimeSpan.Zero;
        private TimeSpan _bootTimeSpan = TimeSpan.Zero;
        private bool _isShovelEmpty = true;
        private bool _isMining;

        public ICommand ConfigControlCenterHost { get; private set; }

        private StateBarViewModel() {
            this.ConfigControlCenterHost = new DelegateCommand(() => {
                ControlCenterHostConfig.ShowWindow();
            });
            VirtualRoot.On<Per1SecondEvent>("挖矿计时秒表，周期性挥动铲子表示在挖矿中", LogEnum.None,
                action: message => {
                    DateTime now = DateTime.Now;
                    this.BootTimeSpan = now - NTMinerRoot.Current.CreatedOn;

                    var mineContext = NTMinerRoot.Current.CurrentMineContext;
                    if (mineContext != null) {
                        this.MineTimeSpan = now - mineContext.CreatedOn;
                    }
                    // 周期性挥动铲子表示在挖矿中
                    if (IsMining) {
                        IsShovelEmpty = !IsShovelEmpty;
                    }
                });
            VirtualRoot.On<MineStartedEvent>("挖矿开始后将风扇转起来", LogEnum.DevConsole,
                action: message => {
                    this.IsMining = true;
                });
            VirtualRoot.On<MineStopedEvent>("挖矿停止后将风扇停转", LogEnum.DevConsole,
                action: message => {
                    this.IsMining = false;
                });
        }

        public bool IsShovelEmpty {
            get => _isShovelEmpty;
            set {
                if (_isShovelEmpty != value) {
                    _isShovelEmpty = value;
                    OnPropertyChanged(nameof(IsShovelEmpty));
                }
            }
        }

        public bool IsMining {
            get => _isMining;
            set {
                _isMining = value;
                OnPropertyChanged(nameof(IsMining));
            }
        }

        public GpuSpeedViewModels GpuSpeedVms {
            get {
                return GpuSpeedViewModels.Current;
            }
        }

        public TimeSpan BootTimeSpan {
            get { return _bootTimeSpan; }
            set {
                if (_bootTimeSpan != value) {
                    _bootTimeSpan = value;
                    OnPropertyChanged(nameof(BootTimeSpan));
                    OnPropertyChanged(nameof(BootTimeSpanText));
                }
            }
        }

        public string BootTimeSpanText {
            get {
                TimeSpan time = new TimeSpan(this.BootTimeSpan.Hours, this.BootTimeSpan.Minutes, this.BootTimeSpan.Seconds);
                if (this.BootTimeSpan.Days > 0) {
                    return $"{this.BootTimeSpan.Days}天{time.ToString()}";
                }
                else {
                    return time.ToString();
                }
            }
        }

        public TimeSpan MineTimeSpan {
            get {
                return _mineTimeSpan;
            }
            set {
                if (_mineTimeSpan != value) {
                    _mineTimeSpan = value;
                    OnPropertyChanged(nameof(MineTimeSpan));
                    OnPropertyChanged(nameof(MineTimeSpanText));
                }
            }
        }

        public string MineTimeSpanText {
            get {
                TimeSpan time = new TimeSpan(this.MineTimeSpan.Hours, this.MineTimeSpan.Minutes, this.MineTimeSpan.Seconds);
                if (this.MineTimeSpan.Days > 0) {
                    return $"{this.MineTimeSpan.Days}天{time.ToString()}";
                }
                else {
                    return time.ToString();
                }
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return GpuStatusBarViewModel.Current;
            }
        }
    }
}
