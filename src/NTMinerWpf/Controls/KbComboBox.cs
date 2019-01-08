using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Controls {
    public class KbComboBox : ComboBox {
        public FrameworkElement Toolbar {
            get { return (FrameworkElement)GetValue(ToolbarProperty); }
            set { SetValue(ToolbarProperty, value); }
        }
        public static readonly DependencyProperty ToolbarProperty =
            DependencyProperty.Register("Toolbar", typeof(FrameworkElement), typeof(KbComboBox), new PropertyMetadata(null));
    }
}
