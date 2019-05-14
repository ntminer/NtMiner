namespace NTMiner.Vms {
    public class MainBarViewModel : ViewModelBase {
        public MainBarViewModel() { }

        public bool IsAutoCloseServices {
            get => NTMinerRegistry.GetIsAutoCloseServices();
            set {
                NTMinerRegistry.SetIsAutoCloseServices(value);
                OnPropertyChanged(nameof(IsAutoCloseServices));
            }
        }
    }
}
