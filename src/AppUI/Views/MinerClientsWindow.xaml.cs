using MahApps.Metro.Controls;
using NTMiner.Bus;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MinerClientsWindow : MetroWindow {
        private static MinerClientsWindow window = null;
        public static void ShowWindow() {
            if (window == null) {
                window = new MinerClientsWindow();
            }
            window.Show();
            if (window.WindowState == WindowState.Minimized) {
                window.WindowState = WindowState.Normal;
            }
            window.Activate();
        }

        public MinerClientsViewModel Vm {
            get {
                return (MinerClientsViewModel)this.DataContext;
            }
        }

        private MinerClientsWindow() {
            InitializeComponent();
            Width = SystemParameters.FullPrimaryScreenWidth * 0.9;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.8;
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

        protected override void OnClosed(EventArgs e) {
            window = null;
            base.OnClosed(e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this?.DragMove();
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
    }
}
