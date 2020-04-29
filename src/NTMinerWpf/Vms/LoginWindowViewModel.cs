namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _loginName;
        private string _password;
        private string _serverHost;
        private bool _isInnerIp;

        public LoginWindowViewModel(string serverHost = null) {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this._loginName = NTMinerRegistry.GetLoginName();
            if (!string.IsNullOrEmpty(serverHost)) {
                this._serverHost = serverHost;
            }
            else {
                this._serverHost = NTMinerRegistry.GetControlCenterAddress();
            }
            this._isInnerIp = Net.IpUtil.IsInnerIp(_serverHost);
        }

        public void ShowMessage(string message, bool isSuccess = false) {
            if (isSuccess) {
                VirtualRoot.Out.ShowSuccess(message);
            }
            else {
                VirtualRoot.Out.ShowError(message, autoHideSeconds: 4);
            }
        }

        public string ServerHost {
            get => _serverHost;
            set {
                _serverHost = value;
                OnPropertyChanged(nameof(ServerHost));
                this.IsInnerIp = Net.IpUtil.IsInnerIp(value);
            }
        }

        public bool IsInnerIp {
            get => _isInnerIp;
            set {
                _isInnerIp = value;
                OnPropertyChanged(nameof(IsInnerIp));
            }
        }

        public string LoginName {
            get => _loginName;
            set {
                if (_loginName != value) {
                    _loginName = value;
                    NTMinerRegistry.SetLoginName(value);
                    OnPropertyChanged(nameof(LoginName));
                }
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }
}
