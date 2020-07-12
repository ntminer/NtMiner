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
        private string _localIps;
        private readonly DateTime _bootTime = DateTime.MinValue;
        private string _cpuPerformanceText = "0 %";
        private string _cpuTemperatureText = "0 ℃";

        public ICommand WindowsAutoLogon { get; private set; }
        public ICommand EnableRemoteDesktop { get; private set; }

        public StateBarViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            UpdateDateTime();
            this.WindowsAutoLogon = new DelegateCommand(() => {
                if (IsAutoAdminLogon) {
                    return;
                }
                VirtualRoot.Execute(new EnableOrDisableWindowsAutoLoginCommand());
            });
            this.EnableRemoteDesktop = new DelegateCommand(() => {
                if (IsRemoteDesktopEnabled) {
                    return;
                }
                VirtualRoot.Execute(new EnableRemoteDesktopCommand());
            });
            _localIps = VirtualRoot.FormatLocalIps(out string _);
            SetCheckUpdateForeground(isLatest: EntryAssemblyInfo.CurrentVersion >= NTMinerContext.ServerVersion);
        }

        public bool IsAutoAdminLogon {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return false;
                }
                return Windows.OS.Instance.IsAutoAdminLogon;
            }
        }

        public string AutoAdminLogonToolTip {
            get {
                if (IsAutoAdminLogon) {
                    return "Windows开机自动登录已启用";
                }
                return "未启用Windows开机自动登录";
            }
        }

        public void RefreshIsRemoteDesktopEnabled() {
            OnPropertyChanged(nameof(IsRemoteDesktopEnabled));
            OnPropertyChanged(nameof(RemoteDesktopToolTip));
        }

        public bool IsRemoteDesktopEnabled {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return false;
                }
                return NTMinerRegistry.GetIsRdpEnabled();
            }
        }

        public string RemoteDesktopToolTip {
            get {
                if (IsRemoteDesktopEnabled) {
                    return "Windows远程桌面已启用";
                }
                return "Windows远程桌面已禁用";
            }
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

        public void UpdateDateTime() {
            DateTime now = DateTime.Now;
            if (_bootTime.Minute != now.Minute || _bootTime == DateTime.MinValue) {
                this.TimeText = now.ToString("H:mm");
            }
            if (_bootTime.Hour != now.Hour || _bootTime == DateTime.MinValue) {
                this.DateText = now.ToString("yyyy/M/d");
            }
        }

        public string CpuPerformanceText {
            get => _cpuPerformanceText;
            set {
                if (_cpuPerformanceText != value) {
                    _cpuPerformanceText = value;
                    OnPropertyChanged(nameof(CpuPerformanceText));
                }
            }
        }

        public string CpuTemperatureText {
            get => _cpuTemperatureText;
            set {
                if (_cpuTemperatureText != value) {
                    _cpuTemperatureText = value;
                    OnPropertyChanged(nameof(CpuTemperatureText));
                }
            }
        }

        public void RefreshLocalIps() {
            LocalIps = VirtualRoot.FormatLocalIps(out string _);
        }

        public void SetCheckUpdateForeground(bool isLatest) {
            if (isLatest) {
                CheckUpdateForeground = AppUtil.GetResource<SolidColorBrush>("BtnBackground");
            }
            else {
                CheckUpdateForeground = WpfUtil.RedBrush;
            }
        }

        public SolidColorBrush CheckUpdateForeground {
            get => _checkUpdateForeground;
            private set {
                _checkUpdateForeground = value;
                OnPropertyChanged(nameof(CheckUpdateForeground));
            }
        }

        public AppRoot.GpuSpeedViewModels GpuSpeedVms { get; private set; } = AppRoot.GpuSpeedVms;

        public string KernelSelfRestartCountText {
            get {
                var mineContext = NTMinerContext.Instance.LockedMineContext;
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
                    return $"{this._bootTimeSpan.Days.ToString()}天{time.ToString()}";
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
                    return $"{this._mineTimeSpan.Days.ToString()}天{time.ToString(@"hh\:mm")}";
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

        public MinerProfileViewModel MinerProfile {
            get {
                return AppRoot.MinerProfileVm;
            }
        }

        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return AppRoot.GpuStatusBarVm;
            }
        }

        public AppRoot.GpuViewModels GpuVms {
            get {
                return AppRoot.GpuVms;
            }
        }

        public string LocalIps {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return "192.168.1.111";
                }
                return _localIps;
            }
            set {
                _localIps = value;
                OnPropertyChanged(nameof(LocalIps));
            }
        }
    }
}
