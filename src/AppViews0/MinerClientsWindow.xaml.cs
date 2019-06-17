using NTMiner.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MinerClientsWindow : BlankWindow {
        private static MinerClientsWindow _instance = null;
        public static MinerClientsWindow ShowWindow() {
            if (_instance == null) {
                _instance = new MinerClientsWindow();
            }
            _instance.Show();
            if (_instance.WindowState == WindowState.Minimized) {
                _instance.WindowState = WindowState.Normal;
            }
            _instance.Activate();
            return _instance;
        }

        public MinerClientsWindowViewModel Vm {
            get {
                return AppContext.Instance.MinerClientsWindowVm;
            }
        }

        private MinerClientsWindow() {
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            this.DataContext = AppContext.Instance.MinerClientsWindowVm;
            InitializeComponent();
            this.On<Per1SecondEvent>("刷新倒计时秒表", LogEnum.None,
                action: message => {
                    var minerClients = Vm.MinerClients.ToArray();
                    if (Vm.CountDown > 0) {
                        Vm.CountDown = Vm.CountDown - 1;
                        foreach (var item in minerClients) {
                            item.OnPropertyChanged(nameof(item.LastActivedOnText));
                        }
                    }
                });
            this.On<Per10SecondEvent>("周期刷新在线客户端列表", LogEnum.DevConsole,
                action: message => {
                    AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
                });
            EventHandler changeNotiCenterWindowLocation = NotiCenterWindow.CreateNotiCenterWindowLocationManager(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += changeNotiCenterWindowLocation;
            AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
            Write.UserLine("小提示：鼠标配合ctrl和shift可以多选矿机", ConsoleColor.Yellow);
        }

        protected override void OnClosing(CancelEventArgs e) {
            VirtualRoot.Execute(new ChangeServerAppSettingsCommand(
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
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e) {
            _instance = null;
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
            OfficialServer.FileUrlService.GetNTMinerFilesAsync(NTMinerAppType.MinerClient, (ntMinerFiles, ex) => {
                Vm.NTMinerFileList = (ntMinerFiles ?? new List<NTMinerFileData>()).OrderByDescending(a => a.GetVersion()).ToList();
            });
            PopUpgrade.IsOpen = !PopUpgrade.IsOpen;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < dg.ColumnHeaderHeight) {
                return;
            }
            if (Vm.SelectedMinerClients != null && Vm.SelectedMinerClients.Length != 0) {
                Vm.SelectedMinerClients[0].RemoteDesktop.Execute(null);
            }
        }

        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e) {
            e.Handled = true;
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = ColumnsShowScrollView.ContentHorizontalOffset - ColumnsShowScrollView.ViewportWidth;
            ColumnsShowScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < ColumnsShowScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = ColumnsShowScrollView.ContentHorizontalOffset + ColumnsShowScrollView.ViewportWidth;
            ColumnsShowScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < ColumnsShowScrollView.ScrollableWidth;
        }
    }
}
