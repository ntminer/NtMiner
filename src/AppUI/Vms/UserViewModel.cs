using NTMiner.User;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class UserViewModel : ViewModelBase, IUser {
        private string _loginName;
        private string _password;
        private string _description;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public UserViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public void Update(IUser data) {
            this.Password = data.Password;
            this.Description = data.Description;
        }

        public UserViewModel(IUser data) : this(data.LoginName) {
            _password = data.Password;
            _description = data.Description;
        }

        public UserViewModel(string loginName) {
            _loginName = loginName;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.LoginName)) {
                    return;
                }
                if (NTMinerRoot.Current.UserSet.Contains(this.LoginName)) {
                    VirtualRoot.Execute(new UpdateUserCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddUserCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.LoginName)) {
                    return;
                }
                UserEdit.ShowWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.LoginName)) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.LoginName}矿池吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveUserCommand(this.LoginName));
                }, icon: "Icon_Confirm");
            });
        }

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
