using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class WindowsAutoLogonViewModel : ViewModelBase {
        public Guid Id { get; private set; } = Guid.NewGuid();
        private string _userName;
        private string _password;

        public ICommand Ok { get; private set; }

        public WindowsAutoLogonViewModel() {
            this.Ok = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password)) {
                    VirtualRoot.Out.ShowError("用户名或密码不能为空");
                    return;
                }
                Windows.OS.Instance.SetAutoLogon(this.UserName, this.Password);
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
        }

        public string UserName {
            get => _userName;
            set {
                if (_userName != value) {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }
        public string Password {
            get => _password;
            set {
                if (_password != value) {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
    }
}
