using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class EventPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
