using System.Windows.Input;

namespace NTMiner.Vms {
    public class LoginWindowViewModel : ViewModelBase {
        private string _loginName;
        private string _password;
        private string _serverHost;
        private bool _isInnerIp;
        public ICommand ShowSignUpPage { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowSignUpPageCommand());
        });
        public ICommand ShowOnlineUpdate { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new UpgradeCommand(string.Empty, null));
        });

        public LoginWindowViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
        }

        public LoginWindowViewModel(string serverHost = null) {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this._loginName = NTMinerRegistry.GetLoginName();
            this._isInnerIp = NTMinerRegistry.GetMinerStudioIsInnerIp();
            if (!string.IsNullOrEmpty(serverHost)) {
                this._serverHost = serverHost;
            }
            else {
                this._serverHost = RpcRoot.OfficialServerAddress;
            }
        }

        public void ShowMessage(string message, int autoHideSeconds = 4, bool isSuccess = false) {
            if (isSuccess) {
                VirtualRoot.Out.ShowSuccess(message);
            }
            else {
                VirtualRoot.Out.ShowError(message, autoHideSeconds: autoHideSeconds);
            }
        }

        public string CurrentVersion {
            get {
                return EntryAssemblyInfo.CurrentVersionStr;
            }
        }

        /// <summary>
        /// 为了让IDE显式的引用计数为0该文件里其它地方直接使用<see cref="EntryAssemblyInfo.CurrentVersionTag"/>
        /// </summary>
        public static string VersionTag {
            get {
                return EntryAssemblyInfo.CurrentVersionTag;
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
                if (_isInnerIp != value) {
                    _isInnerIp = value;
                    if (value) {
                        this._serverHost = NTKeyword.Localhost;
                    }
                    else {
                        this._serverHost = RpcRoot.OfficialServerAddress;
                    }
                    NTMinerRegistry.SetMinerStudioIsInnerIp(value);
                    OnPropertyChanged(nameof(IsInnerIp));
                }
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
