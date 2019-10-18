using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class WorkerEvents : UserControl {
        private WorkerEventsViewModel Vm {
            get {
                return (WorkerEventsViewModel)this.DataContext;
            }
        }

        public WorkerEvents() {
            InitializeComponent();
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
