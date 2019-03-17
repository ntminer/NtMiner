using NTMiner.Vms;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_VirtualMemory",
                CloseVisible = System.Windows.Visibility.Visible,
                Width = 800,
                MinWidth = 450,
                Height = 360,
                MinHeight = 360
            }, ucFactory: (window) => new VirtualMemory(), fixedSize: false);
        }

        private DriveSet Vm {
            get {
                return (DriveSet)this.DataContext;
            }
        }

        public VirtualMemory() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void OpenDrive_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                Drive drive = (Drive)((FrameworkElement)sender).Tag;
                Process.Start(drive.Name);
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
