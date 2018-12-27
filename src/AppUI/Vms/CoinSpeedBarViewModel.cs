namespace NTMiner.Vms {
    public class CoinSpeedBarViewModel : ViewModelBase {
        public static readonly CoinSpeedBarViewModel Current = new CoinSpeedBarViewModel();

        private CoinSpeedBarViewModel() {
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}
