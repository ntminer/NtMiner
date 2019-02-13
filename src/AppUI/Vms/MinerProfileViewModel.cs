using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
        public static readonly MinerProfileViewModel Current = new MinerProfileViewModel();

        private MinerProfileViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
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
        }

        public IMineWork MineWork {
            get {
                return NTMinerRoot.Current.MinerProfile.MineWork;
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
