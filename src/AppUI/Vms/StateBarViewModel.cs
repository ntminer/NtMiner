using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class StateBarViewModel : ViewModelBase {
        private TimeSpan _mineTimeSpan = TimeSpan.Zero;
        private TimeSpan _bootTimeSpan = TimeSpan.Zero;
        private SolidColorBrush _checkUpdateForeground;

        public ICommand ConfigControlCenterHost { get; private set; }

        public StateBarViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.ConfigControlCenterHost = new DelegateCommand(() => {
                ControlCenterHostConfig.ShowWindow();
            });
            if (NTMinerRoot.CurrentVersion.ToString() != NTMinerRoot.ServerVersion) {
                _checkUpdateForeground = new SolidColorBrush(Colors.Red);
            }
            else {
                _checkUpdateForeground = new SolidColorBrush(Colors.Black);
            }
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public SolidColorBrush CheckUpdateForeground {
            get => _checkUpdateForeground;
            set {
                _checkUpdateForeground = value;
                OnPropertyChanged(nameof(CheckUpdateForeground));
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

        public AppContext.MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Current.MinerProfileVm;
            }
        }

        public AppContext.GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return AppContext.Current.GpuStatusBarVm;
            }
        }

        public AppContext.GpuViewModels GpuVms {
            get {
                return AppContext.GpuVms;
            }
        }
    }
}
