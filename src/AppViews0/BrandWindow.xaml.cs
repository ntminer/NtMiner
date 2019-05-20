using NTMiner.Vms;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class BrandWindow : BlankWindow {
        public BrandWindowViewModel Vm {
            get {
                return (BrandWindowViewModel)this.DataContext;
            }
        }

        public BrandWindow() {
            InitializeComponent();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
