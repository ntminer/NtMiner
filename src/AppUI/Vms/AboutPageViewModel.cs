using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public static readonly AboutPageViewModel Current = new AboutPageViewModel();

        private AboutPageViewModel() {
        }

        public Version CurrentVersion => NTMinerRoot.CurrentVersion;

        public int ThisYear => DateTime.Now.Year;
    }
}
