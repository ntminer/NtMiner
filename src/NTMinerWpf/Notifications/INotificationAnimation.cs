using System.Windows;
using System.Windows.Media.Animation;

namespace NTMiner.Notifications {
    /// <summary>
    /// The animation properties for a notification message or some
    /// other item.
    /// </summary>
    public interface INotificationAnimation
    {
        /// <summary>
        /// Gets or sets whether the item animates in and out.
        /// </summary>
        bool Animates { get; set; }

        /// <summary>
        /// Gets or sets the animation in duration (in seconds).
        /// </summary>
        double AnimationInDuration { get; set; }

        /// <summary>
        /// Gets or sets the animation out duration (in seconds).
        /// </summary>
        double AnimationOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the animation in.
        /// </summary>
        AnimationTimeline AnimationIn { get; set; }

        /// <summary>
        /// Gets or sets the animation out.
        /// </summary>
        AnimationTimeline AnimationOut { get; set; }

        /// <summary>
        /// Gets or sets the DependencyProperty for the animation in.
        /// </summary>
        DependencyProperty AnimationInDependencyProperty { get; set; }

        /// <summary>
        /// Gets or sets the DependencyProperty for the animation out.
        /// </summary>
        DependencyProperty AnimationOutDependencyProperty { get; set; }

        /// <summary>
        /// Gets the animatable UIElement.
        /// Typically this is the whole Control object so that the entire
        /// item can be animated.
        /// </summary>
        UIElement AnimatableElement { get; }
    }
}
