using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class EventPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "事件",
                IconName = "Icon_Event",
                CloseVisible = System.Windows.Visibility.Visible,
                Width = 1000,
                Height = 560
            }, ucFactory: (window) => new EventPage(), fixedSize: false);
        }

        private EventPageViewModel Vm {
            get {
                return (EventPageViewModel)this.DataContext;
            }
        }

        public EventPage() {
            InitializeComponent();
        }
    }
}
