using NTMiner.Notifications;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class WindowsLoginViewModel : ViewModelBase {
        private string _userName;
        private string _password;
        private string _message;

        public ICommand Login { get; private set; }

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public WindowsLoginViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public WindowsLoginViewModel(Guid clientId, string minerName, string ip, MinerClientViewModel minerClientVm) {
            this.ClientId = clientId;
            this.MinerName = minerName;
            this.Ip = ip;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password)) {
                    throw new ValidationException("用户名密码不能为空");
                }
                minerClientVm.WindowsLoginName = this.UserName;
                minerClientVm.WindowsPassword = this.Password;
                CloseWindow?.Invoke();
            });
            this.Login = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password)) {
                    throw new ValidationException("用户名密码不能为空");
                }
                minerClientVm.WindowsLoginName = this.UserName;
                minerClientVm.WindowsPassword = this.Password;
                AppHelper.RemoteDesktop?.Invoke(new RemoteDesktopInput(this.Ip, this.UserName, this.Password, this.MinerName, message => {
                    MinerClientsWindowViewModel.Current.Manager.ShowErrorMessage(message);
                }));
                CloseWindow?.Invoke();
            });
        }

        public Guid ClientId { get; private set; }

        public string MinerName { get; private set; }

        public string Ip { get; private set; }

        public string Message {
            get => _message;
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string UserName {
            get => _userName;
            set {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
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
