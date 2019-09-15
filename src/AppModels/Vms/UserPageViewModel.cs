namespace NTMiner.Vms {
    public class UserPageViewModel : ViewModelBase {
        public UserPageViewModel() {
        }

        public AppContext.UserViewModels UserVms {
            get {
                return AppContext.Instance.UserVms;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get { return AppContext.Instance.MinerProfileVm; }
        }

        public bool IsMinerStudio {
            get {
                return VirtualRoot.IsMinerStudio;
            }
        }
    }
}
