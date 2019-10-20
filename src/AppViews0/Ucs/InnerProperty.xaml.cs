using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InnerProperty : UserControl {
        public InnerProperty() {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
