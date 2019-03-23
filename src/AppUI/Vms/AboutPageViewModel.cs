using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public static readonly AboutPageViewModel Current = new AboutPageViewModel();

        private AboutPageViewModel() {
        }

        public Version CurrentVersion {
            get {
                return NTMinerRoot.CurrentVersion;
            }
        }

        public int ThisYear {
            get {
                return DateTime.Now.Year;
            }
        }
    }
}
