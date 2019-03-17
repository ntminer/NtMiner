namespace NTMiner.Vms {
    public class MinerProfileIndexViewModel : ViewModelBase {
        public static readonly MinerProfileIndexViewModel Current = new MinerProfileIndexViewModel();

        private MinerProfileIndexViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
        }

        public Vm Vm {
            get {
                return Vm.Instance;
            }
        }
    }
}
