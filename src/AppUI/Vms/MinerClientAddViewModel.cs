using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientAddViewModel : ViewModelBase {
        private string _minerIp;

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerClientAddViewModel() {
            this.Save = new DelegateCommand(() => {
                Server.ControlCenterService.AddClientAsync(this.ClientIp, (response, e) => {
                    if (!response.IsSuccess()) {
                        if (response != null) {
                            Write.UserLine(response.Description, ConsoleColor.Red);
                            throw new ValidationException(response.Description);
                        }
                    }
                    else {
                        UIThread.Execute(() => {
                            CloseWindow?.Invoke();
                        });
                    }
                });
            });
        }

        public string ClientIp {
            get => _minerIp;
            set {
                _minerIp = value;
                OnPropertyChanged(nameof(ClientIp));
            }
        }
    }
}
