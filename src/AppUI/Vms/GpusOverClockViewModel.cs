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
                data.CoinVm.GpuAllOverClockDataVm.Update(data);
            });
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}
