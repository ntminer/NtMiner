using NTMiner.Hub;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class StartStopMineButtonViewModel : ViewModelBase {
        public static StartStopMineButtonViewModel Instance { get; private set; } = new StartStopMineButtonViewModel();

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private StartStopMineButtonViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.BuildCmdPath<StopMineCommand>(path: message => {
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
            bool isAutoStart = (MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart);
            if (isAutoStart && !this.MinerProfile.IsMining) {
                NTMinerConsole.UserInfo($"{MinerProfile.AutoStartDelaySeconds.ToString()}秒后开始挖矿");
                this.MinerProfile.IsMining = true;
                IMessagePathId pathId = null;
                pathId = VirtualRoot.BuildViaTimesLimitPath<Per1SecondEvent>("自动开始挖矿倒计时", LogEnum.None,
                    path: message => {
                        if (!NTMinerContext.IsAutoStartCanceled) {
                            MineBtnText = $"倒计时{pathId.ViaTimesLimit.ToString()}";
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

        private string _mineBtnText = "正在挖矿";
        public string MineBtnText {
            get => _mineBtnText;
            set {
                _mineBtnText = value;
                OnPropertyChanged(nameof(MineBtnText));
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppRoot.MinerProfileVm;
            }
        }
    }
}
