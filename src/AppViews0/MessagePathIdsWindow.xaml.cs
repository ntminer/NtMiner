using NTMiner.Vms;
using System.ComponentModel;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MessagePathIdsWindow : BlankWindow {
        public MessagePathIdsWindowViewModel Vm {
            get {
                return (MessagePathIdsWindowViewModel)this.DataContext;
            }
        }

        public MessagePathIdsWindow() {
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
