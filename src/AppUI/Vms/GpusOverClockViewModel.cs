using NTMiner.Core;
using NTMiner.Views;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpusOverClockViewModel : ViewModelBase {
        public static readonly GpusOverClockViewModel Current = new GpusOverClockViewModel();

        public ICommand OverClock { get; private set; }

        private GpusOverClockViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.OverClock = new DelegateCommand<OverClockDataViewModel>((data) => {
                DialogWindow.ShowDialog(message: $"确定应用该超频设置吗？", title: "确认", onYes: () => {
                    if (data.CoinVm.CoinProfile.IsOverClockGpuAll) {
                        data.CoinVm.GpuAllOverClockDataVm.Update(data);
                    }
                    else {
                        foreach (var item in data.CoinVm.GpuOverClockVms) {
                            if (item.Index == NTMinerRoot.GpuAllId) {
                                continue;
                            }
                            item.Update(data);
                        }
                    }
                    this.MinerProfile.CoinVm.ApplyOverClock.Execute(null);
                }, icon: IconConst.IconConfirm);
            });
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}
