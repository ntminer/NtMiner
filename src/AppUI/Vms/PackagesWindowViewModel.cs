namespace NTMiner.Vms {
    public class PackagesWindowViewModel : ViewModelBase {
        public AppContext.PackageViewModels PackageVms {
            get {
                return AppContext.Instance.PackageVms;
            }
        }
    }
}
