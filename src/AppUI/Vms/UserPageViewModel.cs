namespace NTMiner.Vms {
    public class UserPageViewModel : ViewModelBase {
        public static readonly UserPageViewModel Current = new UserPageViewModel();

        private UserPageViewModel() {
        }

        public UserViewModels UserVms {
            get {
                return UserViewModels.Current;
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
