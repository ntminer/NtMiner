using NTMiner.User;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class UserViewModel : ViewModelBase, IUser, IEditableViewModel {
        public readonly Guid Id = Guid.NewGuid();

        private string _loginName;
        private string _password;
        private bool _isEnabled;
        private string _description;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }
        public ICommand Enable { get; private set; }
        public ICommand Disable { get; private set; }

        public UserViewModel(string loginName) : this() {
            _loginName = loginName;
        }

        public UserViewModel() {
            this.Save = new DelegateCommand(() => {
                if (!VirtualRoot.IsMinerStudio) {
                    return;
                }
                if (string.IsNullOrEmpty(this.LoginName)) {
                    return;
                }
                IUser user = NTMinerRoot.Instance.UserSet.GetUser(this.LoginName);
                if (user != null) {
                    VirtualRoot.Execute(new UpdateUserCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddUserCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                VirtualRoot.Execute(new UserEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (!VirtualRoot.IsMinerStudio) {
                    return;
                }
                if (string.IsNullOrEmpty(this.LoginName)) {
                    return;
                }
                if (VirtualRoot.IsMinerStudio && this.LoginName == VirtualRoot.RpcUser.LoginName) {
                    throw new ValidationException("不能删除自己");
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.LoginName}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveUserCommand(this.LoginName));
                }));
            });
            this.Enable = new DelegateCommand(() => {
                if (!VirtualRoot.IsMinerStudio) {
                    return;
                }
                if (this.IsEnabled) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定启用{this.LoginName}吗？", title: "确认", onYes: () => {
                    this.IsEnabled = true;
                    VirtualRoot.Execute(new UpdateUserCommand(this));
                }));
            });
            this.Disable = new DelegateCommand(() => {
                if (!VirtualRoot.IsMinerStudio) {
                    return;
                }
                if (!this.IsEnabled) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定禁用{this.LoginName}吗？", title: "确认", onYes: () => {
                    this.IsEnabled = false;
                    VirtualRoot.Execute(new UpdateUserCommand(this));
                }));
            });
        }

        public UserViewModel(IUser data) : this(data.LoginName) {
            _password = data.Password;
            _isEnabled = data.IsEnabled;
            _description = data.Description;
        }

        public void Update(IUser data) {
            this.Password = data.Password;
            this.IsEnabled = data.IsEnabled;
            this.Description = data.Description;
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
                return VirtualRoot.IsMinerStudio;
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private const string _stars = "●●●●●●●●●●";
        private string _passwordStar;
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
                    if (VirtualRoot.IsMinerStudio) {
                        this.Password = HashUtil.Sha1(value);
                    }
                    else {
                        this.Password = HashUtil.Sha1($"{HashUtil.Sha1(HashUtil.Sha1(value))}{VirtualRoot.Id.ToString()}");
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
    }
}
