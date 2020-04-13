using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
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
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                OnOk?.Invoke(this);
            });
        }

        public RemoteDesktopLoginViewModel(string loginName) : this() {
            this._loginName = loginName;
        }

        public string Title {
            get; set;
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
    }
}
