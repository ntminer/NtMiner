using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class StateBarViewModel : ViewModelBase {
        private TimeSpan _mineTimeSpan = TimeSpan.Zero;
        private TimeSpan _bootTimeSpan = TimeSpan.Zero;
        private SolidColorBrush _checkUpdateForeground;
        private Visibility _isNoticeVisible = Visibility.Collapsed;
        private string _poolDelayText;
        private string _dualPoolDelayText;
        private string _timeText;
        private string _dateText;

        public bool IsAutoAdminLogon {
            get { return Windows.OS.Instance.IsAutoAdminLogon; }
        }

        public string TimeText {
            get => _timeText;
            set {
                _timeText = value;
                OnPropertyChanged(nameof(TimeText));
            }
        }
        public string DateText {
            get => _dateText;
            set {
                _dateText = value;
                OnPropertyChanged(nameof(DateText));
            }
        }

        private DateTime _now = DateTime.MinValue;
        public void UpdateDateTime() {
            DateTime now = DateTime.Now;
            if (_now.Minute != now.Minute) {
                this.TimeText = now.ToString("H:mm");
            }
            if (_now.Hour != now.Hour) {
                this.DateText = now.ToString("yyyy/M/d");
            }
        }

        public ICommand ConfigControlCenterHost { get; private set; }

        public StateBarViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            UpdateDateTime();
            this.ConfigControlCenterHost = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowControlCenterHostConfigCommand());
            });
            if (NTMinerRoot.CurrentVersion.ToString() != NTMinerRoot.ServerVersion) {
                _checkUpdateForeground = new SolidColorBrush(Colors.Red);
            }
            else {
                _checkUpdateForeground = new SolidColorBrush(Colors.Black);
            }
        }

        public SolidColorBrush CheckUpdateForeground {
            get => _checkUpdateForeground;
            set {
                _checkUpdateForeground = value;
                OnPropertyChanged(nameof(CheckUpdateForeground));
            }
        }

        public AppContext.GpuSpeedViewModels GpuSpeedVms { get; private set; } = AppContext.Instance.GpuSpeedVms;

        public string KernelSelfRestartCountText {
            get {
                var mineContext = NTMinerRoot.Instance.CurrentMineContext;
                if (mineContext == null || mineContext.KernelSelfRestartCount <= 0) {
                    return string.Empty;
                }
                return mineContext.KernelSelfRestartCount - 1 + "次";
            }
        }

        public void UpdateBootTimeSpan(TimeSpan value) {
            _bootTimeSpan = value;
            OnPropertyChanged(nameof(BootTimeSpanText));
        }

        public string BootTimeSpanText {
            get {
                TimeSpan time = new TimeSpan(this._bootTimeSpan.Hours, this._bootTimeSpan.Minutes, this._bootTimeSpan.Seconds);
                if (this._bootTimeSpan.Days > 0) {
                    return $"{this._bootTimeSpan.Days}天{time.ToString()}";
                }
                else {
                    return time.ToString();
                }
            }
        }

        public void UpdateMineTimeSpan(TimeSpan value) {
            if (value.Minutes != _mineTimeSpan.Minutes) {
                _mineTimeSpan = value;
                OnPropertyChanged(nameof(MineTimeSpanText));
            }
            else {
                _mineTimeSpan = value;
            }
        }

        public string MineTimeSpanText {
            get {
                TimeSpan time = new TimeSpan(this._mineTimeSpan.Hours, this._mineTimeSpan.Minutes, this._mineTimeSpan.Seconds);
                if (this._mineTimeSpan.Days > 0) {
                    return $"{this._mineTimeSpan.Days}天{time.ToString(@"hh\:mm")}";
                }
                else {
                    return time.ToString(@"hh\:mm");
                }
            }
        }

        public Visibility IsNoticeVisible {
            get => _isNoticeVisible;
            set {
                _isNoticeVisible = value;
                OnPropertyChanged(nameof(IsNoticeVisible));
            }
        }

        public string PoolDelayText {
            get { return _poolDelayText; }
            set {
                _poolDelayText = value;
                OnPropertyChanged(nameof(PoolDelayText));
            }
        }

        public string DualPoolDelayText {
            get { return _dualPoolDelayText; }
            set {
                _dualPoolDelayText = value;
                OnPropertyChanged(nameof(DualPoolDelayText));
            }
        }

        public AppContext.MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }

        public AppContext.GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return AppContext.Instance.GpuStatusBarVm;
            }
        }

        public AppContext.GpuViewModels GpuVms {
            get {
                return AppContext.Instance.GpuVms;
            }
        }
    }
}
