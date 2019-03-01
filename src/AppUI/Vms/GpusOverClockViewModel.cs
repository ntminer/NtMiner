using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpusOverClockViewModel : ViewModelBase {
        public static readonly GpusOverClockViewModel Current = new GpusOverClockViewModel();

        public ICommand FillForm { get; private set; }

        private GpusOverClockViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.FillForm = new DelegateCommand<OverClockDataViewModel>((data) => {
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
            });
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}
