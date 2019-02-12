using NTMiner.Notifications;
using NTMiner.Views.Ucs;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ControlCenterWindowViewModel : ViewModelBase {
        public static readonly ControlCenterWindowViewModel Current = new ControlCenterWindowViewModel();

        public ICommand ConfigMinerServerHost { get; private set; }

        private ControlCenterWindowViewModel() {
            this.ConfigMinerServerHost = new DelegateCommand(() => {
                MinerServerHostConfig.ShowWindow();
            });
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        private INotificationMessageManager _manager;
        public INotificationMessageManager Manager {
            get {
                if (_manager == null) {
                    _manager = new NotificationMessageManager();
                }
                return _manager;
            }
        }
    }
}
