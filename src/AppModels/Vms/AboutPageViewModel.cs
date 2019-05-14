using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public AboutPageViewModel() {
        }

        public Version CurrentVersion => NTMinerRoot.CurrentVersion;

        public int ThisYear => DateTime.Now.Year;
    }
}
