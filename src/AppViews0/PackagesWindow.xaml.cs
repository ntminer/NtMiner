using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class PackagesWindow : BlankWindow {
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
                    _instance.ShowWindow(false);
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

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<PackageViewModel>(sender, e);
        }
    }
}
