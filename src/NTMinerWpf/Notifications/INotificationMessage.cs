using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace NTMiner.Notifications {
    /// <summary>
    /// The notification message.
    /// </summary>
    public interface INotificationMessage
    {
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        Brush Background { get; set; }

        /// <summary>
        /// Gets or sets the accent brush.
        /// </summary>
        /// <value>
        /// The accent brush.
        /// </value>
        Brush AccentBrush { get; set; }

        /// <summary>
        /// Gets or sets the badge accent brush.
        /// </summary>
        /// <value>
        /// The badge accent brush.
        /// </value>
        Brush BadgeAccentBrush { get; set; }

        /// <summary>
        /// Gets or sets the badge text.
        /// </summary>
        /// <value>
        /// The badge text.
        /// </value>
        string BadgeText { get; set; }

        /// <summary>
        /// Gets or sets the badge visibility.
        /// </summary>
        /// <value>
        /// The badge visibility.
        /// </value>
        Visibility BadgeVisibility { get; set; }

        /// <summary>
        /// Gets or sets the button accent brush.
        /// </summary>
        /// <value>
        /// The button accent brush.
        /// </value>
        Brush ButtonAccentBrush { get; set; }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        ObservableCollection<object> Buttons { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        string Header { get; set; }

        /// <summary>
        /// Gets or sets the header visibility.
        /// </summary>
        /// <value>
        /// The header visibility.
        /// </value>
        Visibility HeaderVisibility { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the message visibility.
        /// </summary>
        /// <value>
        /// The message visibility.
        /// </value>
        Visibility MessageVisibility { get; set; }

        /// <summary>
        /// Gets or sets the content of the overlay.
        /// </summary>
        /// <value>
        /// The content of the overlay.
        /// </value>
        object OverlayContent { get; set; }

        /// <summary>
        /// Gets or sets the content of the top additional content area.
        /// </summary>
        /// <value>
        /// The content of the top additional content area.
        /// </value>
        object AdditionalContentTop { get; set; }

        /// <summary>
        /// Gets or sets the content of the bottom additional content area.
        /// </summary>
        /// <value>
        /// The additional content.
        /// </value>
        object AdditionalContentBottom { get; set; }

        /// <summary>
        /// Gets or sets the content of the left additional content area.
        /// </summary>
        /// <value>
        /// The additional content.
        /// </value>
        object AdditionalContentLeft { get; set; }

        /// <summary>
        /// Gets or sets the content of the right additional content area.
        /// </summary>
        /// <value>
        /// The additional content.
        /// </value>
        object AdditionalContentRight { get; set; }

        /// <summary>
        /// Gets or sets the content of the center additional content area.
        /// </summary>
        /// <value>
        /// The additional content.
        /// </value>
        object AdditionalContentMain { get; set; }

        /// <summary>
        /// Gets or sets the content of the over badge additional content area.
        /// </summary>
        /// <value>
        /// The additional content.
        /// </value>
        object AdditionalContentOverBadge { get; set; }

        /// <summary>
        /// Gets or sets the brush of the text.
        /// </summary>
        /// <value>
        /// The text brush.
        /// </value>
        Brush Foreground { get; set; }
    }
}