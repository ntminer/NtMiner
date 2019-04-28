namespace NTMiner.Vms {
    public class MainBarViewModel : ViewModelBase {
        public MainBarViewModel() { }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public bool IsAutoCloseServices {
            get => NTMinerRegistry.GetIsAutoCloseServices();
            set {
                NTMinerRegistry.SetIsAutoCloseServices(value);
                OnPropertyChanged(nameof(IsAutoCloseServices));
            }
        }
    }
}
