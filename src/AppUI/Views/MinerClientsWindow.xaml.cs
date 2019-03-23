using MahApps.Metro.Controls;
using NTMiner.Bus;
using NTMiner.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MinerClientsWindow : MetroWindow, IMainWindow {
        private static MinerClientsWindow _sWindow = null;
        public static MinerClientsWindow ShowWindow() {
            if (_sWindow == null) {
                _sWindow = new MinerClientsWindow();
            }
            _sWindow.Show();
            if (_sWindow.WindowState == WindowState.Minimized) {
                _sWindow.WindowState = WindowState.Normal;
            }
            _sWindow.Activate();
            return _sWindow;
        }

        public MinerClientsWindowViewModel Vm {
            get {
                return MinerClientsWindowViewModel.Current;
            }
        }

        private MinerClientsWindow() {
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            InitializeComponent();
            EventHandler ChangeNotiCenterWindowLocation = Wpf.Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += ChangeNotiCenterWindowLocation;
            this.LocationChanged += ChangeNotiCenterWindowLocation;
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            MinerClientsWindowViewModel.Current.QueryMinerClients();
            DelegateHandler<Per10SecondEvent> refreshMinerClients = VirtualRoot.On<Per10SecondEvent>(
                "周期刷新在线客户端列表",
                LogEnum.Console,
                action: message => {
                    MinerClientsWindowViewModel.Current.QueryMinerClients();
                });
            this.Closing += (object sender, System.ComponentModel.CancelEventArgs e)=> {
                VirtualRoot.UnPath(refreshMinerClients);
                VirtualRoot.Execute(new ChangeAppSettingsCommand(
                    new AppSettingData[]{
                        new AppSettingData {
                            Key = "FrozenColumnCount",
                            Value = Vm.FrozenColumnCount
                        },new AppSettingData {
                            Key = "MaxTemp",
                            Value = Vm.MaxTemp
                        },new AppSettingData {
                            Key = "MinTemp",
                            Value = Vm.MinTemp
                        },new AppSettingData {
                            Key = "RejectPercent",
                            Value = Vm.RejectPercent
                        }
                }));
            };
        }

        public void ShowThisWindow() {
            this.Show();
            if (WindowState == WindowState.Minimized) {
                this.WindowState = WindowState.Normal;
            }
            else {
                var oldState = WindowState;
                this.WindowState = WindowState.Minimized;
                this.WindowState = oldState;
            }
            this.Activate();
        }

        protected override void OnClosed(EventArgs e) {
            _sWindow = null;
            base.OnClosed(e);
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void MinerClientsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            Vm.SelectedMinerClients = ((DataGrid)sender).SelectedItems.Cast<MinerClientViewModel>().ToArray();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void MenuItemWork_Click(object sender, RoutedEventArgs e) {
            PopMineWork.IsOpen = !PopMineWork.IsOpen;
        }

        private void MenuItemGroup_Click(object sender, RoutedEventArgs e) {
            PopMinerGroup.IsOpen = !PopMinerGroup.IsOpen;
        }

        private void PopupButton_Click(object sender, RoutedEventArgs e) {
            PopMineWork.IsOpen = PopMinerGroup.IsOpen = PopUpgrade.IsOpen = false;
        }

        private void MenuItemUpgrade_Click(object sender, RoutedEventArgs e) {
            OfficialServer.FileUrlService.GetNTMinerFilesAsync((ntMinerFiles, ex) => {
                Vm.NTMinerFileList = ntMinerFiles ?? new List<NTMinerFileData>();
            });
            PopUpgrade.IsOpen = !PopUpgrade.IsOpen;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (Vm.SelectedMinerClients != null && Vm.SelectedMinerClients.Length != 0) {
                Vm.SelectedMinerClients[0].RemoteDesktop.Execute(null);
            }
        }
    }
}
