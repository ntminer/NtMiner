using System.Windows;

namespace NTMiner.Controls {
    public class KbLinkButton : KbSystemButton {
        public bool KbDisplayLine {
            get { return (bool)GetValue(KbDisplayLineProperty); }
            set { SetValue(KbDisplayLineProperty, value); }
        }

        public static readonly DependencyProperty KbDisplayLineProperty =
            DependencyProperty.Register("KbDisplayLine", typeof(bool), typeof(KbLinkButton), new PropertyMetadata(true));
    }
}
