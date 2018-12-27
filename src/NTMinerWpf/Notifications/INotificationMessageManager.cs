namespace NTMiner.Notifications {
    /// <summary>
    /// The notification message manager.
    /// </summary>
    public interface INotificationMessageManager
    {
        /// <summary>
        /// Occurs when new notification message is queued.
        /// </summary>
        event NotificationMessageManagerEventHandler OnMessageQueued;

        /// <summary>
        /// Occurs when notification message is dismissed.
        /// </summary>
        event NotificationMessageManagerEventHandler OnMessageDismissed;

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>
        /// The factory.
        /// </value>
        INotificationMessageFactory Factory { get; set; }

        /// <summary>
        /// Queues the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Queue(INotificationMessage message);

        /// <summary>
        /// Dismisses the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Dismiss(INotificationMessage message);
    }
}