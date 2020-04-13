using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public AboutPageViewModel() {
        }

        public int ThisYear => DateTime.Now.Year;
    }
}
