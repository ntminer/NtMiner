using System;
using System.Windows.Media.Imaging;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public AboutPageViewModel() {
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public BitmapImage BigLogoImageSource {
            get {
                return IconConst.BigLogoImageSource;
            }
        }

        public Version CurrentVersion => NTMinerRoot.CurrentVersion;

        public int ThisYear => DateTime.Now.Year;
    }
}
