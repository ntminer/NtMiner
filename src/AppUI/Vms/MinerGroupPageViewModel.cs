namespace NTMiner.Vms {
    public class MinerGroupPageViewModel : ViewModelBase {
        public static readonly MinerGroupPageViewModel Current = new MinerGroupPageViewModel();

        private MinerGroupPageViewModel() { }

        public MinerGroupViewModels MinerGroupVms {
            get {
                return MinerGroupViewModels.Current;
            }
        }
    }
}
