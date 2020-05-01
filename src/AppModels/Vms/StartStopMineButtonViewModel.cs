using NTMiner.Hub;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class StartStopMineButtonViewModel : ViewModelBase {
        public static readonly StartStopMineButtonViewModel Instance = new StartStopMineButtonViewModel();

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private StartStopMineButtonViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.AddCmdPath<StopMineCommand>(action: message => {
                if (!NTMinerContext.Instance.IsMining) {
                    this.MinerProfile.IsMining = false;
                }
                NTMinerContext.IsAutoStartCanceled = true;
                NTMinerContext.Instance.StopMineAsync(StopMineReason.LocalUserAction, () => {
                    if (!NTMinerContext.Instance.IsMining) {
                        this.MinerProfile.IsMining = false;
                    }
                });
            }, this.GetType(), LogEnum.DevConsole);
            this.StartMine = new DelegateCommand(() => {
                VirtualRoot.ThisLocalInfo(nameof(StartStopMineButtonViewModel), $"手动开始挖矿", toConsole: true);
                NTMinerContext.Instance.StartMine();
            });
            this.StopMine = new DelegateCommand(() => {
                VirtualRoot.ThisLocalInfo(nameof(StartStopMineButtonViewModel), $"手动停止挖矿", toConsole: true);
                VirtualRoot.Execute(new StopMineCommand());
            });
        }

        public void AutoStart() {
            bool IsAutoStart = (MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart);
            if (IsAutoStart && !this.MinerProfile.IsMining) {
                Write.UserInfo($"{MinerProfile.AutoStartDelaySeconds.ToString()}秒后开始挖矿");
                this.MinerProfile.IsMining = true;
                IMessagePathId pathId = null;
                pathId = VirtualRoot.AddViaTimesLimitPath<Per1SecondEvent>("挖矿倒计时", LogEnum.None,
                    action: message => {
                        if (NTMinerContext.IsAutoStartCanceled) {
                            BtnStopText = $"尚未开始";
                        }
                        else {
                            BtnStopText = $"倒计时{pathId.ViaTimesLimit.ToString()}";
                        }
                        if (pathId.ViaTimesLimit == 0) {
                            if (!NTMinerContext.IsAutoStartCanceled) {
                                VirtualRoot.ThisLocalInfo(nameof(StartStopMineButtonViewModel), $"自动开始挖矿", toConsole: true);
                                NTMinerContext.Instance.StartMine();
                            }
                        }
                    }, location: this.GetType(), viaTimesLimit: MinerProfile.AutoStartDelaySeconds);
            }
        }

        private string _btnStopText = "正在挖矿";
        public string BtnStopText {
            get => _btnStopText;
            set {
                _btnStopText = value;
                OnPropertyChanged(nameof(BtnStopText));
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppRoot.MinerProfileVm;
            }
        }
    }
}
