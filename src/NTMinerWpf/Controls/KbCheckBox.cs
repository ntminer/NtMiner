using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbCheckBox : CheckBox {
        public SolidColorBrush IconColor {
            get { return (SolidColorBrush)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }
        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(SolidColorBrush), typeof(KbCheckBox), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));

        public bool IsShowContent {
            get { return (bool)GetValue(IsShowContentProperty); }
            set { SetValue(IsShowContentProperty, value); }
        }
        public static readonly DependencyProperty IsShowContentProperty =
            DependencyProperty.Register("IsShowContent", typeof(bool), typeof(KbCheckBox), new PropertyMetadata(true));
    }
}