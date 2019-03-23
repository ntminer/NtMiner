using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class GpuProfilesPage : UserControl {
        public static void ShowWindow(GpuProfilesPageViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                IconName = "Icon_OverClock",
                Width = 800,
                Height = 600,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new GpuProfilesPage(vm), fixedSize: true);
        }

        public GpuProfilesPageViewModel Vm {
            get {
                return (GpuProfilesPageViewModel)this.DataContext;
            }
        }

        public GpuProfilesPage(GpuProfilesPageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
