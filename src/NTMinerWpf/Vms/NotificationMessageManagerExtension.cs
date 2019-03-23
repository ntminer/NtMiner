using NTMiner.Notifications;
using System;

namespace NTMiner.Vms {
    public static class NotificationMessageManagerExtension {
        public static NotificationMessageBuilder Warning(this NotificationMessageBuilder builder, string message) {
            builder
                .Accent("#1751C3")
                .Background("#FFCC00")
                .Foreground("Black")
                .HasHeader("提醒");
            builder.SetMessage(message);

            return builder;
        }

        public static NotificationMessageBuilder Error(this NotificationMessageBuilder builder, string message) {
            builder
                .Accent("#1751C3")
                .Background("#F15B19")
                .HasHeader("错误");
            builder.SetMessage(message);

            return builder;
        }

        public static NotificationMessageBuilder Success(this NotificationMessageBuilder builder, string message) {
            builder
                .Accent("#1751C3")
                .Foreground("White")
                .Background("Green")
                .HasHeader("成功");
            builder.SetMessage(message);

            return builder;
        }

        public static void ShowErrorMessage(this INotificationMessageManager manager, string message, int? delaySeconds = null) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(manager);
                builder.Error(message ?? string.Empty);
                if (delaySeconds.HasValue && delaySeconds.Value != 0) {
                    builder
                        .Dismiss()
                        .WithDelay(TimeSpan.FromSeconds(4))
                        .Queue();
                }
                else {
                    builder
                        .Dismiss().WithButton("忽略", null)
                        .Queue();
                }
            });
        }

        public static void ShowInfo(this INotificationMessageManager manager, string message) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(manager);
                builder.Warning(message ?? string.Empty)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(4))
                    .Queue();
            });
        }

        public static void ShowSuccessMessage(this INotificationMessageManager manager, string message) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage(manager);
                builder.Success(message)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(4))
                    .Queue();
            });
        }
    }
}
