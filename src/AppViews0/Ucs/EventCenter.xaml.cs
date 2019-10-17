using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class EventCenter : UserControl {
        private EventCenterViewModel Vm {
            get {
                return (EventCenterViewModel)this.DataContext;
            }
        }

        public EventCenter() {
            InitializeComponent();
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
