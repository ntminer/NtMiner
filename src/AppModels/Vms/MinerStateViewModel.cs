namespace NTMiner.Vms {
    public class MinerStateViewModel : ViewModelBase {
        public MinerStateViewModel() {
            this.StateBarVm = new StateBarViewModel();
        }

        public StateBarViewModel StateBarVm {
            get; private set;
        }
    }
}
