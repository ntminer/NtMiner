using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbSystemButton : Button {
        public KbSystemButton() {
        }

        [Description("窗体系统按钮大小"), Category("KbSkin")]
        public int KbSystemButtonSize {
            get { return (int)GetValue(KbSystemButtonSizeProperty); }
            set { SetValue(KbSystemButtonSizeProperty, value); }
        }
        public static readonly DependencyProperty KbSystemButtonSizeProperty =
            DependencyProperty.Register(nameof(KbSystemButtonSize), typeof(int), typeof(KbSystemButton), new PropertyMetadata(30));


        [Description("窗体系统按钮鼠标悬浮背景颜色"), Category("KbSkin")]
        public SolidColorBrush KbSystemButtonHoverBackground {
            get { return (SolidColorBrush)GetValue(KbSystemButtonHoverBackgroundProperty); }
            set { SetValue(KbSystemButtonHoverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty KbSystemButtonHoverBackgroundProperty =
            DependencyProperty.Register(nameof(KbSystemButtonHoverBackground), typeof(SolidColorBrush), typeof(KbSystemButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 50, 50, 50))));
        

        [Description("窗体系统按钮颜色"), Category("KbSkin")]
        public SolidColorBrush KbSystemButtonForeground {
            get { return (SolidColorBrush)GetValue(KbSystemButtonForegroundProperty); }
            set { SetValue(KbSystemButtonForegroundProperty, value); }
        }
        public static readonly DependencyProperty KbSystemButtonForegroundProperty =
            DependencyProperty.Register(nameof(KbSystemButtonForeground), typeof(SolidColorBrush), typeof(KbSystemButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));
        

        [Description("窗体系统按钮鼠标悬按钮颜色"), Category("KbSkin")]
        public SolidColorBrush KbSystemButtonHoverForeground {
            get { return (SolidColorBrush)GetValue(KbSystemButtonHoverForegroundProperty); }
            set { SetValue(KbSystemButtonHoverForegroundProperty, value); }
        }
        public static readonly DependencyProperty KbSystemButtonHoverForegroundProperty =
            DependencyProperty.Register(nameof(KbSystemButtonHoverForeground), typeof(SolidColorBrush), typeof(KbSystemButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));


        [Description("图标"), Category("KbSkin")]
        public Geometry Icon {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Geometry), typeof(KbSystemButton), new PropertyMetadata(null));
    }
}
