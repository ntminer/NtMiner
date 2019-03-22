namespace NTMiner.Vms {
    public class ColumnsShowPageViewModel : ViewModelBase {
        public ColumnsShowPageViewModel() { }

        public ColumnsShowViewModels ColumnsShowVms {
            get {
                return ColumnsShowViewModels.Current;
            }
        }
    }
}
