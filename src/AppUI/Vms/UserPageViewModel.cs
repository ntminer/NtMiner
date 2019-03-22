namespace NTMiner.Vms {
    public class UserPageViewModel : ViewModelBase {
        public UserPageViewModel() {
            this.UserVms = new UserViewModels();
        }

        public UserViewModels UserVms {
            get; private set;
        }

        public MinerProfileViewModel MinerProfile {
            get { return MinerProfileViewModel.Current; }
        }

        public bool IsControlCenter {
            get {
                return VirtualRoot.IsControlCenter;
            }
        }
    }
}
