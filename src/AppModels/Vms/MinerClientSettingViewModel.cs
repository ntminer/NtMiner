using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientSettingViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        private bool _isAutoBoot;
        private bool _isAutoStart;

        public ICommand Save { get; private set; }

        public MinerClientSettingViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public MinerClientSettingViewModel(MinerClientViewModel[] minerClients) {
            if (minerClients != null && minerClients.Length == 1) {
                _isAutoBoot = minerClients[0].IsAutoBoot;
                _isAutoStart = minerClients[0].IsAutoStart;
            }
            this.Save = new DelegateCommand(() => {
                if (minerClients != null && minerClients.Length != 0) {
                    foreach (var item in minerClients) {
                        Client.NTMinerDaemonService.SetAutoBootStartAsync(item.MinerIp, this.IsAutoBoot, this.IsAutoStart);
                        item.Refresh.Execute(null);
                    }
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
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
