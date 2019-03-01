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
    }
}
