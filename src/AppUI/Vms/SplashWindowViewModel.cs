using System.Windows.Media.Imaging;

namespace NTMiner.Vms {
    public class SplashWindowViewModel : ViewModelBase {
        public BitmapImage BigLogoImageSource {
            get {
                return IconConst.BigLogoImageSource;
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
