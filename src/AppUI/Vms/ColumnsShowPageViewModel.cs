namespace NTMiner.Vms {
    public class ColumnsShowPageViewModel : ViewModelBase {
        public ColumnsShowPageViewModel() { }

        public AppContext.ColumnsShowViewModels ColumnsShowVms {
            get {
                return AppContext.Instance.ColumnsShowVms;
            }
        }
    }
}
