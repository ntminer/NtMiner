using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InnerProperty : UserControl {
        private InnerPropertyViewModel Vm {
            get {
                return (InnerPropertyViewModel)this.DataContext;
            }
        }

        public InnerProperty() {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
