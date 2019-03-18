using NTMiner.Notifications.Controls;

namespace NTMiner.Notifications {
    /// <summary>
    /// The notification message factory.
    /// </summary>
    /// <seealso cref="INotificationMessageFactory" />
    public class NotificationMessageFactory : INotificationMessageFactory {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// Returns new instance of notification message.
        /// </returns>
        public INotificationMessage GetMessage() {
            return new NotificationMessage();
        }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <returns>
        /// Returns new instance of notification message button.
        /// </returns>
        public INotificationMessageButton GetButton() {
            return new NotificationMessageButton();
        }
    }
}