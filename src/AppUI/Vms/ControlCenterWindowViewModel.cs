using NTMiner.Notifications;
using NTMiner.Views.Ucs;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ControlCenterWindowViewModel : ViewModelBase {
        public static readonly ControlCenterWindowViewModel Current = new ControlCenterWindowViewModel();

        private string _title = "开源矿工中控客户端";

        public ICommand ConfigMinerServerHost { get; private set; }

        private ControlCenterWindowViewModel() {
            this.ConfigMinerServerHost = new DelegateCommand(() => {
                MinerServerHostConfig.ShowWindow();
            });
        }

        public string Title {
            get {
                return _title;
            }
            set {
                if (_title != value) {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
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
