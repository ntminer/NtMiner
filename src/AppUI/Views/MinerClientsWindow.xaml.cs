using MahApps.Metro.Controls;
using NTMiner.Bus;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MinerClientsWindow : MetroWindow, IMainWindow {
        private static MinerClientsWindow s_window = null;
        public static MinerClientsWindow ShowWindow() {
            if (s_window == null) {
                s_window = new MinerClientsWindow();
                if (Application.Current.MainWindow == null || Application.Current.MainWindow.GetType() == typeof(SplashWindow)) {
                    Application.Current.MainWindow = s_window;
                }
            }
            s_window.Show();
            if (s_window.WindowState == WindowState.Minimized) {
                s_window.WindowState = WindowState.Normal;
            }
            s_window.Activate();
            return s_window;
        }

        public static bool IsOpened {
            get {
                return s_window != null;
            }
        }

        public MinerClientsWindowViewModel Vm {
            get {
                return (MinerClientsWindowViewModel)this.DataContext;
            }
        }

        private MinerClientsWindow() {
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Vm.QueryMinerClients();
            DelegateHandler<Per10SecondEvent> refreshMinerClients = VirtualRoot.On<Per10SecondEvent>(
                "周期刷新在线客户端列表",
                LogEnum.Console,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.LoadClients();
                    });
                });
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                VirtualRoot.UnPath(refreshMinerClients);
            };
        }

        public void ShowThisWindow() {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        protected override void OnClosed(EventArgs e) {
            s_window = null;
            AppStatic.Managers.RemoveManager(Vm.Manager);
            base.OnClosed(e);
            if (!ChartsWindow.IsOpened) {
                Application.Current.Shutdown();
            }
        }

        private void TbIp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MinerClientViewModel vm = (MinerClientViewModel)((FrameworkElement)sender).Tag;
            vm.RemoteDesktop.Execute(null);
            e.Handled = true;
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                ((MinerClientViewModel)dg.SelectedItem).Details.Execute(null);
            }
        }
    }
}
