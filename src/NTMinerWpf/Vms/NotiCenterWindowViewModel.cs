using NTMiner.Notifications;

namespace NTMiner.Vms {
    public class NotiCenterWindowViewModel : ViewModelBase {
        public static readonly NotiCenterWindowViewModel Current = new NotiCenterWindowViewModel();

        private NotiCenterWindowViewModel() { }

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
