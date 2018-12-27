using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelPage : MetroWindow {
        private static double _left;
        private static double _top;

        private static KernelPage _window;
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete = null) {
            if (_window == null) {
                _window = new KernelPage();
            }
            _window.Show();
            _window.Activate();
            if (kernelId != Guid.Empty) {
                KernelPageViewModel vm = (KernelPageViewModel)_window.DataContext;
                vm.Download(kernelId, (isSuccess, message) => {
                    if (isSuccess) {
                        _window.Close();
                    }
                    downloadComplete(isSuccess, message);
                });
            }
        }

        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        public KernelPage() {
            InitializeComponent();
            if (_left != 0) {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                this.Left = _left;
                this.Top = _top;
            }
        }

        protected override void OnClosed(EventArgs e) {
            _left = this.Left;
            _top = this.Top;
            _window = null;
            base.OnClosed(e);
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void BtnDownloadMenu_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (Vm.KernelDownloadingVisible == System.Windows.Visibility.Visible) {
                Vm.KernelDownloadingVisible = System.Windows.Visibility.Collapsed;
            }
            else {
                Vm.KernelDownloadingVisible = System.Windows.Visibility.Visible;
            }
        }
    }
}
