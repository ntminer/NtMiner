using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class RemoteDesktopLoginViewModel : ViewModelBase {
        private string _loginName;
        private string _password;

        public ICommand Ok { get; private set; }

        public Action CloseWindow { get; set; }

        public Action<RemoteDesktopLoginViewModel> OnOk;

        public RemoteDesktopLoginViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Ok = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(LoginName)) {
                    VirtualRoot.Out.ShowError("登录名不能为空", delaySeconds: 4);
                    return;
                }
                CloseWindow?.Invoke();
                OnOk?.Invoke(this);
            });
        }

        public string Ip {
            get; set;
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
        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }
}
