using NTMiner.User;

namespace NTMiner.Vms {
    public class UserViewModel : ViewModelBase, IUser {
        private string _loginName;
        private string _password;
        private string _description;

        public string LoginName {
            get => _loginName;
            set {
                _loginName = value;
                OnPropertyChanged(nameof(LoginName));
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Description {
            get => _description;
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }
}
