namespace NTMiner.Vms {
    public class UserPageViewModel : ViewModelBase {
        public UserPageViewModel() {
        }

        public AppContext.UserViewModels UserVms {
            get {
                return AppContext.Current.UserVms;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get { return MinerProfileViewModel.Current; }
        }

        public bool IsMinerStudio {
            get {
                return VirtualRoot.IsMinerStudio;
            }
        }
    }
}
