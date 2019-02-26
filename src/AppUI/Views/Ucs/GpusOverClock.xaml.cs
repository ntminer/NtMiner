using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class GpusOverClock : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_OverClock",
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed,
            }, ucFactory: (window) => new GpusOverClock(), fixedSize: true);
        }

        public GpusOverClockViewModel Vm {
            get {
                return (GpusOverClockViewModel)this.DataContext;
            }
        }

        public GpusOverClock() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == typeof(ScrollViewer)) {
                Window.GetWindow(this).DragMove();
                e.Handled = true;
            }
        }
    }
}
