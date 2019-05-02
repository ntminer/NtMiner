namespace NTMiner.Vms {
    public class ConsoleViewModel : ViewModelBase {
        public ConsoleViewModel() {
        }

        public AppContext.MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Current.MinerProfileVm;
            }
        }
    }
}
