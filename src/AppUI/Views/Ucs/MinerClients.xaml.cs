using NTMiner.Bus;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinerClients : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Miner",
                Width = 1300,
                Height = 760,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new MinerClients(window), fixedSize: false);
        }

        public MinerClientsViewModel Vm {
            get {
                return (MinerClientsViewModel)this.DataContext;
            }
        }

        private readonly ContainerWindow _window;
        private MinerClients(ContainerWindow window) {
            _window = window;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Vm.QueryMinerClients();
            DelegateHandler<Per10SecondEvent> refreshMinerClients = VirtualRoot.Access<Per10SecondEvent>(
                Guid.Parse("D0B01F1E-764A-4B83-B115-F7FC496CEB0A"),
                "周期刷新在线客户端列表",
                LogEnum.Console,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.LoadClients();
                    });
                });
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                VirtualRoot.UnAccess(refreshMinerClients);
            };
        }

        private void MinerName_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MinerClientViewModel minerClientVm = (MinerClientViewModel)((FrameworkElement)sender).Tag;
            minerClientVm.ShowReName.Execute(null);
        }

        private void MinerGroup_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MinerClientViewModel minerClientVm = (MinerClientViewModel)((FrameworkElement)sender).Tag;
            minerClientVm.ShowChangeGroup.Execute(null);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                _window?.DragMove();
            }
        }
    }
}
