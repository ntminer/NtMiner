using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class AutoBootStartSeterViewModel : ViewModelBase {
        private bool _isAutoBoot;
        private bool _isAutoStart;

        public Action CloseWindow { get; set; }

        public ICommand Save { get; private set; }

        public AutoBootStartSeterViewModel(MinerClientViewModel[] minerClients) {
            this.Save = new DelegateCommand(() => {
                if (minerClients != null && minerClients.Length != 0) {
                    foreach (var item in minerClients) {
                        Client.NTMinerDaemonService.SetAutoBootStartAsync(item.MinerIp, this.IsAutoStart, this.IsAutoStart);
                    }
                }
                CloseWindow?.Invoke();
            });
        }

        public bool IsAutoBoot {
            get => _isAutoBoot;
            set {
                _isAutoBoot = value;
                OnPropertyChanged(nameof(IsAutoBoot));
            }
        }
        public bool IsAutoStart {
            get => _isAutoStart;
            set {
                _isAutoStart = value;
                OnPropertyChanged(nameof(IsAutoStart));
            }
        }
    }
}
