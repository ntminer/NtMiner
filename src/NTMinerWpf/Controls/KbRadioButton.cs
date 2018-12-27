using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Controls {
    public class KbRadioButton : RadioButton {
        public SolidColorBrush IconColor {
            get { return (SolidColorBrush)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(SolidColorBrush), typeof(KbRadioButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));
    }
}
