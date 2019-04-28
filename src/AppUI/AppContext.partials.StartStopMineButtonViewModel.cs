using NTMiner.Vms;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class StartStopMineButtonViewModel : ViewModelBase {
            public ICommand StartMine { get; private set; }
            public ICommand StopMine { get; private set; }

            public StartStopMineButtonViewModel() {
                this.StartMine = new DelegateCommand(() => {
                    this.MinerProfile.IsMining = true;
                    NTMinerRoot.Current.StartMine();
                    BtnStopText = "正在挖矿";
                });
                this.StopMine = new DelegateCommand(() => {
                    if (!NTMinerRoot.Current.IsMining) {
                        this.MinerProfile.IsMining = false;
                    }
                    NTMinerRoot.IsAutoStartCanceled = true;
                    NTMinerRoot.Current.StopMineAsync();
                });
                if (NTMinerRoot.IsAutoStart && !this.MinerProfile.IsMining) {
                    this.MinerProfile.IsMining = true;
                    int n = 10;
                    Bus.IDelegateHandler handler = null;
                    handler = VirtualRoot.On<Per1SecondEvent>("挖矿倒计时", LogEnum.None,
                    action: message => {
                        BtnStopText = $"倒计时({--n})";
                        if (n <= 0) {
                            BtnStopText = "正在挖矿";
                            VirtualRoot.UnPath(handler);
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
                    return Current.MinerProfileVms;
                }
            }
        }
    }
}
