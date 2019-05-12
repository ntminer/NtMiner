using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class PackagesWindow : MetroWindow {
        private static readonly object _locker = new object();
        private static PackagesWindow _instance = null;
        public static void ShowWindow() {
            UIThread.Execute(() => {
                if (_instance == null) {
                    lock (_locker) {
                        if (_instance == null) {
                            _instance = new PackagesWindow();
                            _instance.Show();
                        }
                    }
                }
                else {
                    AppHelper.ShowWindow(_instance, false);
                }
            });
        }

        public PackagesWindowViewModel Vm {
            get {
                return (PackagesWindowViewModel)this.DataContext;
            }
        }

        public PackagesWindow() {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            _instance = null;
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
