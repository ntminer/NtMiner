using NTMiner.Vms;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class StartStopMineButtonViewModel : ViewModelBase {
            public static readonly StartStopMineButtonViewModel Instance = new StartStopMineButtonViewModel();

            public ICommand StartMine { get; private set; }
            public ICommand StopMine { get; private set; }

            private StartStopMineButtonViewModel() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
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
                    NTMinerRoot.Instance.StopMineAsync(()=> {
                        if (!NTMinerRoot.Instance.IsMining) {
                            this.MinerProfile.IsMining = false;
                        }
                    });
                });
                if (NTMinerRoot.IsAutoStart && !this.MinerProfile.IsMining && VirtualRoot.SecondCount < 10) {
                    this.MinerProfile.IsMining = true;
                    int n = 10 - VirtualRoot.SecondCount;
                    Bus.IDelegateHandler handler = null;
                    handler = On<Per1SecondEvent>("挖矿倒计时", LogEnum.None,
                    action: message => {
                        BtnStopText = $"倒计时({--n})";
                        if (n <= 0) {
                            BtnStopText = "正在挖矿";
                            VirtualRoot.UnPath(handler);
                        }
                    });
                }
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
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
}
