using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public AboutPageViewModel() {
        }

        public Version CurrentVersion => EntryAssemblyInfo.CurrentVersion;

        public int ThisYear => DateTime.Now.Year;
    }
}
