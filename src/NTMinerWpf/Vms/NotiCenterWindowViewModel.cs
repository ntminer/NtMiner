using NTMiner.Notifications;

namespace NTMiner.Vms {
    public class NotiCenterWindowViewModel : ViewModelBase, IShowMessage {
        public static readonly NotiCenterWindowViewModel Instance = new NotiCenterWindowViewModel();
        public static bool IsHotKeyEnabled = false;

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
        public void ShowErrorMessage(string message, int? delaySeconds = null) {
            Manager.ShowErrorMessage(message, delaySeconds);
        }

        public void ShowInfo(string message) {
            Manager.ShowInfo(message);
        }

        public void ShowSuccessMessage(string message, string header = "成功") {
            Manager.ShowSuccessMessage(message, header);
        }
    }
}
