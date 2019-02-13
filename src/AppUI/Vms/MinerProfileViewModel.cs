using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Linq;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
        public static readonly MinerProfileViewModel Current = new MinerProfileViewModel();

        private TimeSpan _mineTimeSpan = TimeSpan.Zero;
        private TimeSpan _bootTimeSpan = TimeSpan.Zero;
        private double _logoRotateTransformAngle;

        private MinerProfileViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            VirtualRoot.Access<Per1SecondEvent>(
                Guid.Parse("479A35A1-5A5A-48AF-B184-F1EC568BE181"),
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
            VirtualRoot.Access<MinerProfilePropertyChangedEvent>(
                Guid.Parse("00F1C9F7-ADC8-438D-8B7E-942F6EE5F9A4"),
                "MinerProfile设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });
            VirtualRoot.Access<MineWorkPropertyChangedEvent>(
                Guid.Parse("7F96F755-E292-4146-9390-75635D150A4B"),
                "MineWork设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });
            VirtualRoot.Access<MineStartedEvent>(
                Guid.Parse("36B6B69F-37E3-44BE-9CA9-20D6764E7058"),
                "挖矿开始后刷新MinerProfileVM的IsMinig属性",
                LogEnum.Console,
                action: message => {
                    this.OnPropertyChanged(nameof(this.IsMining));
                });
            VirtualRoot.Access<MineStopedEvent>(
                Guid.Parse("C4C1308A-1C04-4094-91A2-D11993C626A0"),
                "挖矿停止后刷新MinerProfileVM的IsMinig属性",
                LogEnum.Console,
                action: message => {
                    this.OnPropertyChanged(nameof(this.IsMining));
                });

            VirtualRoot.Access<RefreshArgsAssemblyCommand>(
                Guid.Parse("4931C5C3-178F-4867-B615-215F0744C1EB"),
                "刷新参数总成",
                LogEnum.Console,
                action: cmd => {
                    this.OnPropertyChanged(nameof(this.ArgsAssembly));
                });
            System.Timers.Timer t = new System.Timers.Timer(100);
            t.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
                if (this._logoRotateTransformAngle > 3600000) {
                    this._logoRotateTransformAngle = 0;
                }
                this.LogoRotateTransformAngle += 50;
            };
            VirtualRoot.Access<MineStartedEvent>(
                Guid.Parse("D8CC83D6-B9B5-4739-BD66-0F772A22BBF8"),
                "挖矿开始后将风扇转起来",
                LogEnum.Console,
                action: message => {
                    t.Start();
                    OnPropertyChanged(nameof(GpuStateColor));
                });
            VirtualRoot.Access<MineStopedEvent>(
                Guid.Parse("0BB360E4-92A6-4629-861F-B3E4C9BE1203"),
                "挖矿停止后将风扇停转",
                LogEnum.Console,
                action: message => {
                    t.Stop();
                    OnPropertyChanged(nameof(GpuStateColor));
                });
        }

        public IMineWork MineWork {
            get {
                return NTMinerRoot.Current.MinerProfile.MineWork;
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
                if (IsMining) {
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

        public Guid Id {
            get { return NTMinerRoot.Current.MinerProfile.GetId(); }
        }

        public Guid GetId() {
            return this.Id;
        }

        public string MinerName {
            get => NTMinerRoot.Current.MinerProfile.MinerName;
            set {
                if (NTMinerRoot.Current.MinerProfile.MinerName != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(MinerName), value);
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsAutoThisPCName {
            get {
                return NTMinerRoot.Current.MinerProfile.IsAutoThisPCName;
            }
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoThisPCName != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoThisPCName), value);
                    OnPropertyChanged(nameof(IsAutoThisPCName));
                    if (value) {
                        OnPropertyChanged(nameof(MinerName));
                    }
                }
            }
        }

        public bool IsShowInTaskbar {
            get => NTMinerRoot.Current.MinerProfile.IsShowInTaskbar;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsShowInTaskbar != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsShowInTaskbar), value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }

        public bool IsMining {
            get {
                return NTMinerRoot.Current.IsMining;
            }
        }

        public string ArgsAssembly {
            get {
                return NTMinerRoot.Current.BuildAssembleArgs();
            }
        }

        public bool IsAutoBoot {
            get => NTMinerRoot.Current.MinerProfile.IsAutoBoot;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoBoot != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoBoot), value);
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }
        }

        public string IsAutoBootText {
            get {
                if (IsAutoBoot) {
                    return "是";
                }
                return "否";
            }
        }

        public bool IsNoShareRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsNoShareRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsNoShareRestartKernel != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsNoShareRestartKernel), value);
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public int NoShareRestartKernelMinutes {
            get => NTMinerRoot.Current.MinerProfile.NoShareRestartKernelMinutes;
            set {
                if (NTMinerRoot.Current.MinerProfile.NoShareRestartKernelMinutes != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(NoShareRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsPeriodicRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsPeriodicRestartKernel != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsPeriodicRestartKernel), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public int PeriodicRestartKernelHours {
            get => NTMinerRoot.Current.MinerProfile.PeriodicRestartKernelHours;
            set {
                if (NTMinerRoot.Current.MinerProfile.PeriodicRestartKernelHours != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(PeriodicRestartKernelHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get => NTMinerRoot.Current.MinerProfile.IsPeriodicRestartComputer;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsPeriodicRestartComputer != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsPeriodicRestartComputer), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int PeriodicRestartComputerHours {
            get => NTMinerRoot.Current.MinerProfile.PeriodicRestartComputerHours;
            set {
                if (NTMinerRoot.Current.MinerProfile.PeriodicRestartComputerHours != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(PeriodicRestartComputerHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public bool IsAutoStart {
            get => NTMinerRoot.Current.MinerProfile.IsAutoStart;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoStart != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoStart), value);
                    OnPropertyChanged(nameof(IsAutoStart));
                }
            }
        }

        public bool IsAutoRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsAutoRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoRestartKernel != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsAutoRestartKernel), value);
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public bool IsShowCommandLine {
            get { return NTMinerRoot.Current.MinerProfile.IsShowCommandLine; }
            set {
                if (NTMinerRoot.Current.MinerProfile.IsShowCommandLine != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(IsShowCommandLine), value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        public Guid CoinId {
            get => NTMinerRoot.Current.MinerProfile.CoinId;
            set {
                if (NTMinerRoot.Current.MinerProfile.CoinId != value) {
                    NTMinerRoot.Current.SetMinerProfileProperty(nameof(CoinId), value);
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                CoinViewModel coinVm;
                if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out coinVm)) {
                    coinVm = CoinViewModels.Current.AllCoins.FirstOrDefault();
                    if (coinVm != null) {
                        CoinId = coinVm.Id;
                    }
                }
                if (coinVm != null && !coinVm.IsCurrentCoin) {
                    foreach (var item in CoinViewModels.Current.AllCoins) {
                        item.IsCurrentCoin = false;
                    }
                    coinVm.IsCurrentCoin = true;
                }
                return coinVm;
            }
            set {
                if (value != null && !string.IsNullOrEmpty(value.Code)) {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }
    }
}
