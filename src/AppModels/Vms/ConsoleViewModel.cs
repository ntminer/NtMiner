namespace NTMiner.Vms {
    public class ConsoleViewModel : ViewModelBase {
        public ConsoleViewModel() {
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }
    }
}
