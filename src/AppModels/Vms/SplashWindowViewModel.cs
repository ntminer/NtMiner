namespace NTMiner.Vms {
    public class SplashWindowViewModel : ViewModelBase {
        public string Version {
            get {
                return $"v{MainAssemblyInfo.CurrentVersion.ToString()}({MainAssemblyInfo.CurrentVersionTag})";
            }
        }
        public string WindowTitle { get; private set; } = "NTMiner";
    }
}
