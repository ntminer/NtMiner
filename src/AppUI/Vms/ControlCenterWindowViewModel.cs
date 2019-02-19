using NTMiner.Notifications;
using NTMiner.Views.Ucs;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ControlCenterWindowViewModel : ViewModelBase {
        public static readonly ControlCenterWindowViewModel Current = new ControlCenterWindowViewModel();

        private INotificationMessageManager _manager;
        private double _height;
        private double _width;

        public ICommand ConfigMinerServerHost { get; private set; }

        private ControlCenterWindowViewModel() {
            this.Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            this.Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            this.ConfigMinerServerHost = new DelegateCommand(() => {
                MinerServerHostConfig.ShowWindow();
            });
        }

        public double Height {
            get => _height;
            set => _height = value;
        }

        public double Width {
            get => _width;
            set => _width = value;
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

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
