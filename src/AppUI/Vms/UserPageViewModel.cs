namespace NTMiner.Vms {
    public class UserPageViewModel : ViewModelBase {
        public static readonly UserPageViewModel Current = new UserPageViewModel();

        private UserPageViewModel() {
        }

        public Vm Vm {
            get {
                return Vm.Instance;
            }
        }

        public bool IsControlCenter {
            get {
                return VirtualRoot.IsControlCenter;
            }
        }
    }
}
