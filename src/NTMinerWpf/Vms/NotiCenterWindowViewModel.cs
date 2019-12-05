using NTMiner.Notifications;
using System;

namespace NTMiner.Vms {
    public class NotiCenterWindowViewModel : ViewModelBase, IOut {
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
        public void ShowError(string message, int delaySeconds) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(Manager);
                builder.Error(message ?? string.Empty);
                if (delaySeconds != 0) {
                    builder
                        .Dismiss()
                        .WithDelay(TimeSpan.FromSeconds(delaySeconds))
                        .Queue();
                }
                else {
                    builder
                        .Dismiss().WithButton("忽略", null)
                        .Queue();
                }
            });
        }

        public void ShowWarn(string message, int delaySeconds) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(Manager);
                builder.Warning(message ?? string.Empty);
                if (delaySeconds != 0) {
                    builder
                        .Dismiss()
                        .WithDelay(TimeSpan.FromSeconds(delaySeconds))
                        .Queue();
                }
                else {
                    builder
                        .Dismiss().WithButton("忽略", null)
                        .Queue();
                }
            });
        }

        public void ShowInfo(string message, int delaySeconds) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(Manager);
                builder.Warning(message ?? string.Empty)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(delaySeconds))
                    .Queue();
            });
        }

        public void ShowSuccess(string message, int delaySeconds, string header = "成功") {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(Manager);
                builder.Success(header, message)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(delaySeconds))
                    .Queue();
            });
        }
    }
}
