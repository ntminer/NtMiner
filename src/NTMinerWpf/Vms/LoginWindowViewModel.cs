using System.Windows;

namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _hostAndPort;
        private string _loginName;
        private string _message;
        private Visibility _messageVisible = Visibility.Collapsed;

        public LoginWindowViewModel() {
            this._hostAndPort = $"{Server.MinerServerHost}:{Server.MinerServerPort.ToString()}";
            this._loginName = "admin";
        }

        public string HostAndPort {
            get => _hostAndPort;
            set {
                _hostAndPort = value;
                OnPropertyChanged(nameof(HostAndPort));
            }
        }

        public string LoginName {
            get => _loginName;
            set {
                _loginName = value;
                OnPropertyChanged(nameof(LoginName));
            }
        }
        public string Message {
            get => _message;
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public Visibility MessageVisible {
            get => _messageVisible;
            set {
                _messageVisible = value;
                OnPropertyChanged(nameof(MessageVisible));
            }
        }
    }
}
