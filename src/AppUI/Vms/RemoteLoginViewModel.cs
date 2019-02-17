using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class RemoteLoginViewModel : ViewModelBase {
        private string _userName;
        private string _password;

        public ICommand Login { get; private set; }

        public Action CloseWindow { get; set; }

        public RemoteLoginViewModel(Guid clientId, string minerName, string ip) {
            this.ClientId = clientId;
            this.MinerName = minerName;
            this.Ip = ip;
            this.Login = new DelegateCommand(() => {
                Server.ControlCenterService.UpdateClientAsync(this.ClientId, nameof(ClientDataViewModel.RemoteUserName), this.UserName, response => {

                    CloseWindow?.Invoke();
                });
            });
        }

        public Guid ClientId { get; private set; }

        public string MinerName { get; private set; }

        public string Ip { get; private set; }

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
