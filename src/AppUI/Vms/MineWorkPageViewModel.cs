namespace NTMiner.Vms {
    public class MineWorkPageViewModel : ViewModelBase {
        public static readonly MineWorkPageViewModel Current = new MineWorkPageViewModel();

        private MineWorkPageViewModel() {
        }

        public MineWorkViewModels MineWorkVms {
            get {
                return MineWorkViewModels.Current;
            }
        }
    }
}
