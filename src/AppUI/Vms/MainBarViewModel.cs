namespace NTMiner.Vms {
    public class MainBarViewModel : ViewModelBase {
        public static readonly MainBarViewModel Current = new MainBarViewModel();

        private MainBarViewModel() { }

        public bool IsAutoCloseServices {
            get => NTMinerRegistry.GetIsAutoCloseServices();
            set {
                NTMinerRegistry.SetIsAutoCloseServices(value);
                OnPropertyChanged(nameof(IsAutoCloseServices));
            }
        }
    }
}
