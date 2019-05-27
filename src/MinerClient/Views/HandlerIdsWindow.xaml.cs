using NTMiner.Vms;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class HandlerIdsWindow : BlankWindow {
        public HandlerIdsWindowViewModel Vm {
            get {
                return (HandlerIdsWindowViewModel)this.DataContext;
            }
        }

        public HandlerIdsWindow() {
            InitializeComponent();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
