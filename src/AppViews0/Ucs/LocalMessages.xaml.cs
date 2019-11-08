using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class LocalMessages : UserControl {
        private LocalMessagesViewModel Vm {
            get {
                return (LocalMessagesViewModel)this.DataContext;
            }
        }

        public LocalMessages() {
            InitializeComponent();
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }
    }
}
