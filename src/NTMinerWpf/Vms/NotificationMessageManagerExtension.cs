using NTMiner.Notifications;
using System;

namespace NTMiner.Vms {
    public static class NotificationMessageManagerExtension {
        public static NotificationMessageBuilder Warning(this NotificationMessageBuilder builder, string message) {
            builder
                .Accent("#1751C3")
                .Background("#333")
                .HasHeader("信息");
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

        public static void ShowErrorMessage(this INotificationMessageManager manager, string message) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage();
                builder.Manager = manager;
                builder.Message = manager.Factory.GetMessage();
                builder.Error(message ?? string.Empty)
                    .Dismiss().WithButton("忽略", null)
                    .Queue();
            });
        }

        public static void ShowMessage(this INotificationMessageManager manager, string message) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage();
                builder.Manager = manager;
                builder.Message = manager.Factory.GetMessage();
                builder.Warning(message ?? string.Empty)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(4))
                    .Queue();
            });
        }

        public static void ShowSuccessMessage(this INotificationMessageManager manager, string message) {
            UIThread.Execute(() => {
                var builder = NotificationMessageBuilder.CreateMessage();
                builder.Manager = manager;
                builder.Message = manager.Factory.GetMessage();
                builder.Success(message)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(4))
                    .Queue();
            });
        }
    }
}
