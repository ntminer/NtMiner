namespace NTMiner.Vms {
    public class MainMenuViewModel : ViewModelBase {
        public MainMenuViewModel() { }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }
    }
}
