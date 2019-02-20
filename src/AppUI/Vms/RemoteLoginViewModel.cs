using NTMiner.Notifications;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class RemoteLoginViewModel : ViewModelBase {
        private string _userName;
        private string _password;
        private string _message;

        public ICommand Login { get; private set; }

        public Action CloseWindow { get; set; }

        public RemoteLoginViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public RemoteLoginViewModel(Guid clientId, string minerName, string ip, MinerClientViewModel minerClientVm) {
            this.ClientId = clientId;
            this.MinerName = minerName;
            this.Ip = ip;
            this.Login = new DelegateCommand(() => {
                Server.ControlCenterService.UpdateClientPropertiesAsync(this.ClientId, new Dictionary<string, object> {
                    { nameof(MinerClientViewModel.RemoteUserName), this.UserName },
                    { nameof(MinerClientViewModel.RemotePassword), this.Password }
                }, response => {
                    if (response.IsSuccess()) {
                        minerClientVm.RemoteUserName = this.UserName;
                        minerClientVm.RemotePassword = this.Password;
                        VirtualRoot.RemoteDesktop.OpenRemoteDesktop(this.Ip, this.UserName, this.Password, this.MinerName, onDisconnected: message=> {
                            MinerClientsViewModel.Current.Manager.CreateMessage()
                                .Accent("#1751C3")
                                .Background("Red")
                                .HasBadge("Error")
                                .HasMessage(message)
                                .Dismiss()
                                .WithDelay(TimeSpan.FromSeconds(5))
                                .Queue();
                        });
                        UIThread.Execute(() => {
                            CloseWindow?.Invoke();
                        });
                    }
                    else {
                        if (response != null) {
                            this.Message = response.Description;
                            TimeSpan.FromSeconds(3).Delay().ContinueWith(t => {
                                this.Message = string.Empty;
                            });
                        }
                    }
                });
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
