using NTMiner.User;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class UserViewModel : ViewModelBase, IUser, ICanUpdateByReflection {
        public readonly Guid Id = Guid.NewGuid();

        private string _loginName;
        private string _password;
        private bool _isEnabled;
        private string _description;
        private const string _stars = "●●●●●●●●●●";
        private string _passwordStar;
        private string _email;
        private string _mobile;
        private string _publicKey;
        private string _privateKey;
        private string _roles;
        private DateTime _createdOn;

        public ICommand Enable { get; private set; }
        public ICommand Disable { get; private set; }

        public UserViewModel(string loginName) : this() {
            _loginName = loginName;
        }

        public UserViewModel() {
            this.Enable = new DelegateCommand(() => {
                if (!ClientAppType.IsMinerStudio) {
                    return;
                }
                if (this.IsEnabled) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定启用{this.LoginName}吗？", title: "确认", onYes: () => {
                    RpcRoot.OfficialServer.UserService.EnableUserAsync(this.LoginName, (response, exception) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.RaiseEvent(new UserEnabledEvent(Guid.Empty, this));
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(exception), autoHideSeconds: 4);
                        }
                    });
                }));
            });
            this.Disable = new DelegateCommand(() => {
                if (!ClientAppType.IsMinerStudio) {
                    return;
                }
                if (!this.IsEnabled) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定禁用{this.LoginName}吗？", title: "确认", onYes: () => {
                    RpcRoot.OfficialServer.UserService.DisableUserAsync(this.LoginName, (response, exception) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.RaiseEvent(new UserDisabledEvent(Guid.Empty, this));
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(exception), autoHideSeconds: 4);
                        }
                    });
                }));
            });
        }

        public UserViewModel(IUser data) : this(data.LoginName) {
            _password = data.Password;
            _isEnabled = data.IsEnabled;
            _description = data.Description;
            _email = data.Email;
            _mobile = data.Mobile;
            _publicKey = data.PublicKey;
            _privateKey = data.PrivateKey;
            _roles = data.Roles;
            _createdOn = data.CreatedOn;
        }

        public string LoginName {
            get => _loginName;
            set {
                _loginName = value;
                OnPropertyChanged(nameof(LoginName));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("登录名不能为空");
                }
            }
        }

        public bool IsReadOnly {
            get {
                return !string.IsNullOrEmpty(this.LoginName);
            }
        }

        public bool IsMinerStudio {
            get {
                return ClientAppType.IsMinerStudio;
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string PasswordStar {
            get {
                if (string.IsNullOrEmpty(this.Password)) {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(_passwordStar)) {
                    return _stars;
                }
                return _passwordStar;
            }
            set {
                if (_passwordStar != value) {
                    _passwordStar = value;
                    OnPropertyChanged(nameof(PasswordStar));
                    if (ClientAppType.IsMinerStudio) {
                        this.Password = HashUtil.Sha1(value);
                    }
                    else {
                        this.Password = HashUtil.Sha1($"{HashUtil.Sha1(HashUtil.Sha1(value))}{NTMinerContext.Id.ToString()}");
                    }
                }
            }
        }

        public bool IsEnabled {
            get { return _isEnabled; }
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(IsEnabledText));
            }
        }

        public string IsEnabledText {
            get {
                return this.IsEnabled ? "已启用" : "已禁用";
            }
        }

        public string Description {
            get => _description;
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Email {
            get => _email;
            set {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Mobile {
            get => _mobile;
            set {
                _mobile = value;
                OnPropertyChanged(nameof(Mobile));
            }
        }

        public string PublicKey {
            get {
                return _publicKey;
            }
            set {
                _publicKey = value;
                OnPropertyChanged(nameof(PublicKey));
            }
        }

        public string PrivateKey {
            get {
                return _privateKey;
            }
            set {
                _privateKey = value;
                OnPropertyChanged(nameof(PrivateKey));
            }
        }

        public string Roles {
            get => _roles;
            set {
                _roles = value;
                OnPropertyChanged(nameof(Roles));
            }
        }

        public DateTime CreatedOn {
            get => _createdOn;
            set {
                _createdOn = value;
                OnPropertyChanged(nameof(CreatedOn));
            }
        }
    }
}
