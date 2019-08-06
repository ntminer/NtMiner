using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbButton : Button {
        [Description("窗体系统按钮鼠标悬浮背景颜色"), Category("KbSkin")]
        public SolidColorBrush KbButtonHoverBackground {
            get { return (SolidColorBrush)GetValue(KbButtonHoverBackgroundProperty); }
            set { SetValue(KbButtonHoverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty KbButtonHoverBackgroundProperty =
            DependencyProperty.Register("KbButtonHoverBackground", typeof(SolidColorBrush), typeof(KbButton), new PropertyMetadata(Wpf.Util.TransparentBrush));

        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(KbButton), new PropertyMetadata(null));
    }
}
