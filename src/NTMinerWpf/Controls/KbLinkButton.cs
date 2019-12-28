using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Controls {
    public class KbLinkButton : Button {
        public bool KbDisplayLine {
            get { return (bool)GetValue(KbDisplayLineProperty); }
            set { SetValue(KbDisplayLineProperty, value); }
        }

        public static readonly DependencyProperty KbDisplayLineProperty =
            DependencyProperty.Register(nameof(KbDisplayLine), typeof(bool), typeof(KbLinkButton), new PropertyMetadata(true));
    }
}
