using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class RemoteDesktopLoginViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();

        private string _loginName;
        private string _password;

        public ICommand Ok { get; private set; }

        public Action<RemoteDesktopLoginViewModel> OnOk;

        public RemoteDesktopLoginViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Ok = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(LoginName)) {
                    VirtualRoot.Out.ShowError("登录名不能为空", autoHideSeconds: 4);
                    return;
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
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
