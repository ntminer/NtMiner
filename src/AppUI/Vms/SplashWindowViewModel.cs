namespace NTMiner.Vms {
    public class SplashWindowViewModel : ViewModelBase {
        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public string Version {
            get {
                return $"NTMiner v{NTMinerRoot.CurrentVersion.ToString()}({NTMinerRoot.CurrentVersionTag})";
            }
        }
        public string WindowTitle { get; private set; } = "NTMiner";
    }
}
