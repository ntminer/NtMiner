using NTMiner.Bus;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class StartStopMineButtonViewModel : ViewModelBase {
        public static readonly StartStopMineButtonViewModel Instance = new StartStopMineButtonViewModel();

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        private StartStopMineButtonViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
#if DEBUG
                Write.Stopwatch.Restart();
#endif
            this.StartMine = new DelegateCommand(() => {
                this.MinerProfile.IsMining = true;
                NTMinerRoot.Instance.StartMine();
                BtnStopText = "正在挖矿";
            });
            this.StopMine = new DelegateCommand(() => {
                if (!NTMinerRoot.Instance.IsMining) {
                    this.MinerProfile.IsMining = false;
                }
                NTMinerRoot.IsAutoStartCanceled = true;
                NTMinerRoot.Instance.StopMineAsync(StopMineReason.LocalUserAction, () => {
                    if (!NTMinerRoot.Instance.IsMining) {
                        this.MinerProfile.IsMining = false;
                    }
                });
            });
#if DEBUG
                Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        public void AutoStart() {
            bool IsAutoStart = (MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart);
            if (IsAutoStart && !this.MinerProfile.IsMining) {
                this.MinerProfile.IsMining = true;
                int n = MinerProfile.AutoStartDelaySeconds;
                IHandlerId handler = null;
                handler = AppContext.EventPath<Per1SecondEvent>("挖矿倒计时", LogEnum.None,
                action: message => {
                    if (NTMinerRoot.IsAutoStartCanceled) {
                        BtnStopText = $"尚未开始";
                        n = 0;
                    }
                    else {
                        BtnStopText = $"倒计时{--n}";
                    }
                    if (n <= 0) {
                        VirtualRoot.UnPath(handler);
                        if (!NTMinerRoot.IsAutoStartCanceled) {
                            BtnStopText = "正在挖矿";
                            MinerProfile.IsMining = true;
                            NTMinerRoot.Instance.StartMine();
                        }
                    }
                });
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
                return AppContext.Instance.MinerProfileVm;
            }
        }
    }
}
