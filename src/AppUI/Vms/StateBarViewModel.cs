using NTMiner.Views.Ucs;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class StateBarViewModel : ViewModelBase {
        public static readonly StateBarViewModel Current = new StateBarViewModel();

        private TimeSpan _mineTimeSpan = TimeSpan.Zero;
        private TimeSpan _bootTimeSpan = TimeSpan.Zero;
        private double _logoRotateTransformAngle;

        public ICommand ConfigMinerServerHost { get; private set; }

        private StateBarViewModel() {
            this.ConfigMinerServerHost = new DelegateCommand(() => {
                MinerServerHostConfig.ShowWindow();
            });
            VirtualRoot.On<Per1SecondEvent>(
                "挖矿计时秒表",
                LogEnum.None,
                action: message => {
                    DateTime now = DateTime.Now;
                    this.BootTimeSpan = now - NTMinerRoot.Current.CreatedOn;

                    var mineContext = NTMinerRoot.Current.CurrentMineContext;
                    if (mineContext != null) {
                        this.MineTimeSpan = now - mineContext.CreatedOn;
                    }
                });
            System.Timers.Timer t = new System.Timers.Timer(100);
            t.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
                if (this._logoRotateTransformAngle > 3600000) {
                    this._logoRotateTransformAngle = 0;
                }
                this.LogoRotateTransformAngle += 50;
            };
            VirtualRoot.On<MineStartedEvent>(
                "挖矿开始后将风扇转起来",
                LogEnum.Console,
                action: message => {
                    t.Start();
                    this.OnPropertyChanged(nameof(this.IsMining));
                    OnPropertyChanged(nameof(GpuStateColor));
                });
            VirtualRoot.On<MineStopedEvent>(
                "挖矿停止后将风扇停转",
                LogEnum.Console,
                action: message => {
                    t.Stop();
                    this.OnPropertyChanged(nameof(this.IsMining));
                    OnPropertyChanged(nameof(GpuStateColor));
                });
        }

        public bool IsMining {
            get {
                return NTMinerRoot.Current.IsMining;
            }
        }

        public GpuSpeedViewModels GpuSpeedVms {
            get {
                return GpuSpeedViewModels.Current;
            }
        }

        private static readonly SolidColorBrush Gray = new SolidColorBrush(Colors.Gray);
        private static readonly SolidColorBrush MiningColor = (SolidColorBrush)System.Windows.Application.Current.Resources["IconFillColor"];
        public SolidColorBrush GpuStateColor {
            get {
                if (NTMinerRoot.Current.IsMining) {
                    return MiningColor;
                }
                return Gray;
            }
        }

        public double LogoRotateTransformAngle {
            get => _logoRotateTransformAngle;
            set {
                if (_logoRotateTransformAngle != value) {
                    _logoRotateTransformAngle = value;
                    OnPropertyChanged(nameof(LogoRotateTransformAngle));
                }
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

        public Version CurrentVersion {
            get {
                return NTMinerRoot.CurrentVersion;
            }
        }

        public string VersionTag {
            get {
                return NTMinerRoot.CurrentVersionTag;
            }
        }

        public string QQGroup {
            get {
                return NTMinerRoot.Current.QQGroup;
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

        public string WindowsEdition {
            get {
                return Windows.OS.Current.WindowsEdition;
            }
        }

        public string TotalVirtualMemoryGbText {
            get {
                return DriveSet.Current.VirtualMemorySet.TotalVirtualMemoryGbText;
            }
        }

        public string GpuSetInfo {
            get {
                return NTMinerRoot.Current.GpuSetInfo;
            }
        }

        public string DriverVersion {
            get {
                var driverVersion = NTMinerRoot.Current.GpuSet.Properties.FirstOrDefault(a => a.Code == "DriverVersion");
                if (driverVersion == null || driverVersion.Value == null) {
                    return string.Empty;
                }
                return driverVersion.Value.ToString();
            }
        }
    }
}
