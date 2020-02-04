using NTMiner.Vms;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "虚拟内存",
                IconName = "Icon_VirtualMemory",
                CloseVisible = Visibility.Visible,
                Width = 800,
                MinWidth = 450,
                Height = 360,
                MinHeight = 360,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new VirtualMemory(), fixedSize: false);
        }

        private AppContext.DriveSetViewModel Vm {
            get {
                return AppContext.Instance.DriveSetVm;
            }
        }

        public VirtualMemory() {
            this.DataContext = AppContext.Instance.DriveSetVm;
            InitializeComponent();
        }

        private void OpenDrive_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                DriveViewModel drive = (DriveViewModel)((FrameworkElement)sender).Tag;
                Process.Start(drive.Name);
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
