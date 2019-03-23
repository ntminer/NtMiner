using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NTMiner.Notifications.Controls {
    /// <summary>
    /// The notification message control.
    /// </summary>
    /// <seealso cref="INotificationMessage" />
    /// <seealso cref="Control" />
    public class NotificationMessage : Control, INotificationMessage, INotificationAnimation {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessage" /> class.
        /// </summary>
        public NotificationMessage() {
            this.Buttons = new ObservableCollection<object>();

            // Setting the default text color, if not defined by user.
            this.Foreground = new BrushConverter().ConvertFromString("#DDDDDD") as Brush;
        }

        /// <summary>
        /// Gets or sets the content of the overlay.
        /// </summary>
        /// <value>
        /// The content of the overlay.
        /// </value>
        public object OverlayContent {
            get => GetValue(OverlayContentProperty);
            set => SetValue(OverlayContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the top additional content area.
        /// </summary>
        /// <value>
        /// The content of the top additional content area.
        /// </value>
        public object AdditionalContentTop {
            get => GetValue(AdditionalContentTopProperty);
            set => SetValue(AdditionalContentTopProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the bottom additional content area.
        /// </summary>
        /// <value>
        /// The content of the bottom additional content area.
        /// </value>
        public object AdditionalContentBottom {
            get => GetValue(AdditionalContentBottomProperty);
            set => SetValue(AdditionalContentBottomProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the left additional content area.
        /// </summary>
        /// <value>
        /// The content of the left additional content area.
        /// </value>
        public object AdditionalContentLeft {
            get => GetValue(AdditionalContentLeftProperty);
            set => SetValue(AdditionalContentLeftProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the right additional content area.
        /// </summary>
        /// <value>
        /// The content of the right additional content area.
        /// </value>
        public object AdditionalContentRight {
            get => GetValue(AdditionalContentRightProperty);
            set => SetValue(AdditionalContentRightProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the center additional content area.
        /// </summary>
        /// <value>
        /// The content of the center additional content area.
        /// </value>
        public object AdditionalContentMain {
            get => GetValue(AdditionalContentMainProperty);
            set => SetValue(AdditionalContentMainProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the top additional content area.
        /// </summary>
        /// <value>
        /// The content of the top additional content area.
        /// </value>
        public object AdditionalContentOverBadge {
            get => GetValue(AdditionalContentOverBadgeProperty);
            set => SetValue(AdditionalContentOverBadgeProperty, value);
        }

        /// <summary>
        /// Gets or sets the accent brush.
        /// </summary>
        /// <value>
        /// The accent brush.
        /// </value>
        public Brush AccentBrush {
            get => (Brush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the button accent brush.
        /// </summary>
        /// <value>
        /// The button accent brush.
        /// </value>
        public Brush ButtonAccentBrush {
            get => (Brush)GetValue(ButtonAccentBrushProperty);
            set => SetValue(ButtonAccentBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the badge visibility.
        /// </summary>
        /// <value>
        /// The badge visibility.
        /// </value>
        public Visibility BadgeVisibility {
            get => (Visibility)GetValue(BadgeVisibilityProperty);
            set => SetValue(BadgeVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the badge accent brush.
        /// </summary>
        /// <value>
        /// The badge accent brush.
        /// </value>
        public Brush BadgeAccentBrush {
            get => (Brush)GetValue(BadgeAccentBrushProperty);
            set => SetValue(BadgeAccentBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the badge text.
        /// </summary>
        /// <value>
        /// The badge text.
        /// </value>
        public string BadgeText {
            get => (string)GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the header visibility.
        /// </summary>
        /// <value>
        /// The header visibility.
        /// </value>
        public Visibility HeaderVisibility {
            get => (Visibility)GetValue(HeaderVisibilityProperty);
            set => SetValue(HeaderVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public string Header {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the message visibility.
        /// </summary>
        /// <value>
        /// The message visibility.
        /// </value>
        public Visibility MessageVisibility {
            get => (Visibility)GetValue(MessageVisibilityProperty);
            set => SetValue(MessageVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        public ObservableCollection<object> Buttons {
            get => (ObservableCollection<object>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the message animates.
        /// </summary>
        /// <value>
        /// Whether or not the message animates.
        /// </value>
        public bool Animates {
            get => (bool)GetValue(AnimatesProperty);
            set => SetValue(AnimatesProperty, value);
        }

        /// <summary>
        /// Gets or sets how long the message animates in (in seconds).
        /// </summary>
        /// <value>
        /// How long the message animates in (in seconds).
        /// </value>
        public double AnimationInDuration {
            get => (double)GetValue(AnimationInDurationProperty);
            set => SetValue(AnimationInDurationProperty, value);
        }

        /// <summary>
        /// Gets or sets how long the message animates out (in seconds).
        /// </summary>
        /// <value>
        /// How long the message animates out (in seconds).
        /// </value>
        public double AnimationOutDuration {
            get => (double)GetValue(AnimationOutDurationProperty);
            set => SetValue(AnimationOutDurationProperty, value);
        }

        /// <summary>
        /// The animatable element used for show/hide animations.
        /// </summary>
        public UIElement AnimatableElement => this;

        /// <summary>
        /// The animation in.
        /// </summary>
        public AnimationTimeline AnimationIn {
            get {
                var animation = (AnimationTimeline)GetValue(AnimationInProperty);
                if (animation != null) {
                    animation.Duration = TimeSpan.FromSeconds(AnimationInDuration);
                    return animation;
                }
                else {
                    var doubleAnimation = new DoubleAnimation {
                        From = 0,
                        To = 1,
                        BeginTime = TimeSpan.FromSeconds(0),
                        Duration = TimeSpan.FromSeconds(AnimationInDuration),
                        FillBehavior = FillBehavior.HoldEnd
                    };
                    return doubleAnimation;
                }
            }
            set => SetValue(AnimationInProperty, value);
        }

        /// <summary>
        /// The animation out.
        /// </summary>
        public AnimationTimeline AnimationOut {
            get {
                var animation = (AnimationTimeline)GetValue(AnimationOutProperty);
                if (animation != null) {
                    animation.Duration = TimeSpan.FromSeconds(AnimationOutDuration);
                    return animation;
                }
                else {
                    return new DoubleAnimation {
                        From = 1,
                        To = 0,
                        BeginTime = TimeSpan.FromSeconds(0),
                        Duration = TimeSpan.FromSeconds(AnimationOutDuration),
                        FillBehavior = FillBehavior.HoldEnd
                    };
                }
            }
            set => SetValue(AnimationOutProperty, value);
        }

        /// <summary>
        /// The dependency property on which to animate while animating in.
        /// </summary>
        public DependencyProperty AnimationInDependencyProperty {
            get {
                var property = (DependencyProperty)GetValue(AnimationInDependencyPropProperty);
                return property ?? OpacityProperty;
            }
            set => SetValue(AnimationInDependencyPropProperty, value);
        }

        /// <summary>
        /// The dependency property on which to animate while animating out.
        /// </summary>
        public DependencyProperty AnimationOutDependencyProperty {
            get {
                var property = (DependencyProperty)GetValue(AnimationOutDependencyPropProperty);
                return property ?? OpacityProperty;
            }
            set => SetValue(AnimationOutDependencyPropProperty, value);
        }

        /// <summary>
        /// The overlay content property.
        /// </summary>
        public static readonly DependencyProperty OverlayContentProperty =
            DependencyProperty.Register("OverlayContent", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The additional content top property.
        /// </summary>
        public static readonly DependencyProperty AdditionalContentTopProperty =
            DependencyProperty.Register("AdditionalContentTop", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The additional content bottom property.
        /// </summary>
        public static readonly DependencyProperty AdditionalContentBottomProperty =
            DependencyProperty.Register("AdditionalContentBottom", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The additional content left property.
        /// </summary>
        public static readonly DependencyProperty AdditionalContentLeftProperty =
            DependencyProperty.Register("AdditionalContentLeft", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The additional content right property.
        /// </summary>
        public static readonly DependencyProperty AdditionalContentRightProperty =
            DependencyProperty.Register("AdditionalContentRight", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The additional content main property.
        /// </summary>
        public static readonly DependencyProperty AdditionalContentMainProperty =
            DependencyProperty.Register("AdditionalContentMain", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The additional content over badge property.
        /// </summary>
        public static readonly DependencyProperty AdditionalContentOverBadgeProperty =
            DependencyProperty.Register("AdditionalContentOverBadge", typeof(object), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The accent brush property.
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(NotificationMessage), new PropertyMetadata(null, AccentBrushPropertyChangedCallback));

        /// <summary>
        /// Accents the brush property changed callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void AccentBrushPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            if (!(dependencyObject is NotificationMessage @this))
                throw new NullReferenceException("Dependency object is not of valid type " + nameof(NotificationMessage));

            if (@this.BadgeAccentBrush == null) {
                @this.BadgeAccentBrush = dependencyPropertyChangedEventArgs.NewValue as Brush;
            }

            if (@this.ButtonAccentBrush == null) {
                @this.ButtonAccentBrush = dependencyPropertyChangedEventArgs.NewValue as Brush;
            }
        }

        /// <summary>
        /// The button accent brush property.
        /// </summary>
        public static readonly DependencyProperty ButtonAccentBrushProperty =
            DependencyProperty.Register("ButtonAccentBrush", typeof(Brush), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The badge visibility property.
        /// </summary>
        public static readonly DependencyProperty BadgeVisibilityProperty =
            DependencyProperty.Register("BadgeVisibility", typeof(Visibility), typeof(NotificationMessage), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// The badge accent brush property.
        /// </summary>
        public static readonly DependencyProperty BadgeAccentBrushProperty =
            DependencyProperty.Register("BadgeAccentBrush", typeof(Brush), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The badge text property.
        /// </summary>
        public static readonly DependencyProperty BadgeTextProperty =
            DependencyProperty.Register("BadgeText", typeof(string), typeof(NotificationMessage), new PropertyMetadata(null, BadgeTextPropertyChangedCallback));

        /// <summary>
        /// Badges the text property changed callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void BadgeTextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            if (!(dependencyObject is NotificationMessage @this))
                throw new NullReferenceException("Dependency object is not of valid type " + nameof(NotificationMessage));

            @this.BadgeVisibility = dependencyPropertyChangedEventArgs.NewValue == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// The header visibility property.
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(NotificationMessage), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// The header property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(NotificationMessage), new PropertyMetadata(null, HeaderPropertyChangesCallback));

        /// <summary>
        /// Headers the property changes callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void HeaderPropertyChangesCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            if (!(dependencyObject is NotificationMessage @this))
                throw new NullReferenceException("Dependency object is not of valid type " + nameof(NotificationMessage));

            @this.HeaderVisibility = dependencyPropertyChangedEventArgs.NewValue == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// The message visibility property.
        /// </summary>
        public static readonly DependencyProperty MessageVisibilityProperty =
            DependencyProperty.Register("MessageVisibility", typeof(Visibility), typeof(NotificationMessage), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// The message property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(NotificationMessage), new PropertyMetadata(null, MessagePropertyChangesCallback));

        /// <summary>
        /// Messages the property changes callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void MessagePropertyChangesCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            if (!(dependencyObject is NotificationMessage @this))
                throw new NullReferenceException("Dependency object is not of valid type " + nameof(NotificationMessage));

            @this.MessageVisibility = dependencyPropertyChangedEventArgs.NewValue == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// The buttons property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(ObservableCollection<object>), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The animates property.
        /// </summary>
        public static readonly DependencyProperty AnimatesProperty =
            DependencyProperty.Register("Animates", typeof(bool), typeof(NotificationMessage), new PropertyMetadata(false));

        /// <summary>
        /// The animation in duration property (in seconds).
        /// </summary>
        public static readonly DependencyProperty AnimationInDurationProperty =
            DependencyProperty.Register("AnimationInDuration", typeof(double), typeof(NotificationMessage), new PropertyMetadata(0.25));

        /// <summary>
        /// The animation out duration property (in seconds).
        /// </summary>
        public static readonly DependencyProperty AnimationOutDurationProperty =
            DependencyProperty.Register("AnimationOutDuration", typeof(double), typeof(NotificationMessage), new PropertyMetadata(0.25));

        /// <summary>
        /// The animation in property.
        /// </summary>
        public static readonly DependencyProperty AnimationInProperty =
            DependencyProperty.Register("AnimationIn", typeof(AnimationTimeline), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The animation out property.
        /// </summary>
        public static readonly DependencyProperty AnimationOutProperty =
            DependencyProperty.Register("AnimationOut", typeof(AnimationTimeline), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The animation in dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationInDependencyPropProperty =
            DependencyProperty.Register("AnimationInDependencyProperty", typeof(DependencyProperty), typeof(NotificationMessage), new PropertyMetadata(null));

        /// <summary>
        /// The animation out dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationOutDependencyPropProperty =
            DependencyProperty.Register("AnimationOutDependencyProperty", typeof(DependencyProperty), typeof(NotificationMessage), new PropertyMetadata(null));


        /// <summary>
        /// Initializes the <see cref="NotificationMessage" /> class.
        /// </summary>
        static NotificationMessage() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationMessage), new FrameworkPropertyMetadata(typeof(NotificationMessage)));
        }
    }
}
