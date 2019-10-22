using NTMiner.Vms;
using System.ComponentModel;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class PathIdsWindow : BlankWindow {
        public PathIdsWindowViewModel Vm {
            get {
                return (PathIdsWindowViewModel)this.DataContext;
            }
        }

        public PathIdsWindow() {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            base.OnClosing(e);
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
