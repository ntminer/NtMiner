namespace NTMiner.Vms {
    public class OverClockDataPageViewModel : ViewModelBase {
        public static readonly OverClockDataPageViewModel Current = new OverClockDataPageViewModel();

        private OverClockDataPageViewModel() { }

        public OverClockDataViewModels OverClockDataVms {
            get {
                return OverClockDataViewModels.Current;
            }
        }
    }
}
