namespace NTMiner.Vms {
    public class ColumnsShowPageViewModel : ViewModelBase {
        public static readonly ColumnsShowPageViewModel Current = new ColumnsShowPageViewModel();

        private ColumnsShowPageViewModel() { }

        public ColumnsShowViewModels ColumnsShowVms {
            get {
                return ColumnsShowViewModels.Current;
            }
        }
    }
}
