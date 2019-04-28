using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class StateBarViewModel : ViewModelBase {
        private TimeSpan _mineTimeSpan = TimeSpan.Zero;
        private TimeSpan _bootTimeSpan = TimeSpan.Zero;

        public ICommand ConfigControlCenterHost { get; private set; }

        public StateBarViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.ConfigControlCenterHost = new DelegateCommand(() => {
                ControlCenterHostConfig.ShowWindow();
            });
            VirtualRoot.On<Per1SecondEvent>("挖矿计时秒表，周期性挥动铲子表示在挖矿中", LogEnum.None,
                action: message => {
                    DateTime now = DateTime.Now;
                    this.BootTimeSpan = now - NTMinerRoot.Instance.CreatedOn;
                    if (NTMinerRoot.IsAutoStart && VirtualRoot.SecondCount <= 10 && !NTMinerRoot.IsAutoStartCanceled) {
                        return;
                    }
                    var mineContext = NTMinerRoot.Instance.CurrentMineContext;
                    if (mineContext != null) {
                        this.MineTimeSpan = now - mineContext.CreatedOn;
                        if (!this.MinerProfile.IsMining) {
                            this.MinerProfile.IsMining = true;
                        }
                    }
                    else {
                        if (this.MinerProfile.IsMining) {
                            this.MinerProfile.IsMining = false;
                        }
                    }
                });
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public AppContext.GpuSpeedViewModels GpuSpeedVms { get; private set; } = AppContext.Current.GpuSpeedVms;

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
                return AppContext.Current.MinerProfileVms;
            }
        }

        public AppContext.GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return AppContext.Current.GpuStatusBarVms;
            }
        }

        public AppContext.GpuViewModels GpuVms {
            get {
                return AppContext.GpuVms;
            }
        }
    }
}
