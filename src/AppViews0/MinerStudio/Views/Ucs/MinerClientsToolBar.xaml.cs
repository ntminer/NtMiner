using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerClientsToolBar : UserControl {
        public MinerClientsWindowViewModel Vm {
            get {
                return MinerStudioRoot.MinerClientsWindowVm;
            }
        }

        public MinerClientsToolBar() {
            this.DataContext = Vm;
            InitializeComponent();
        }

        private void MenuItemWork_Click(object sender, RoutedEventArgs e) {
            var popup = PopupMineWork;
            MineWorkViewModel selected = null;
            if (Vm.SelectedMinerClients.Length == 1) {
                selected = Vm.SelectedMinerClients[0].SelectedMineWork;
            }
            if (popup.Child == null) {
                popup.Child = new MineWorkSelect(new MineWorkSelectViewModel("为选中的矿机分配作业", selected, selectedResult => {
                    foreach (var item in Vm.SelectedMinerClients) {
                        item.SelectedMineWork = selectedResult;
                    }
                    popup.IsOpen = false;
                    MenuItemMineWork.Visibility = Visibility.Visible;
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                        MenuItemMineWork.Visibility = Visibility.Visible;
                    })
                });
            }
            else {
                ((MineWorkSelect)popup.Child).Vm.SelectedResult = selected;
            }
            popup.IsOpen = true;
        }

        private void MenuItemGroup_Click(object sender, RoutedEventArgs e) {
            var popup = PopupMinerGroup;
            MinerGroupViewModel selected = null;
            if (Vm.SelectedMinerClients.Length == 1) {
                selected = Vm.SelectedMinerClients[0].SelectedMinerGroup;
            }
            if (popup.Child == null) {
                popup.Child = new MinerGroupSelect(new MinerGroupSelectViewModel("将选中的矿机放进分组", selected, selectedResult => {
                    foreach (var item in Vm.SelectedMinerClients) {
                        item.SelectedMinerGroup = selectedResult;
                    }
                    popup.IsOpen = false;
                    MenuItemMinerGroup.Visibility = Visibility.Visible;
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                        MenuItemMinerGroup.Visibility = Visibility.Visible;
                    })
                });
            }
            else {
                ((MinerGroupSelect)popup.Child).Vm.SelectedResult = selected;
            }
            popup.IsOpen = true;
        }

        private void PopupMineWork_Opened(object sender, System.EventArgs e) {
            MenuItemMineWork.Visibility = Visibility.Collapsed;
        }

        private void PopupMineWork_Closed(object sender, System.EventArgs e) {
            TimeSpan.FromMilliseconds(100).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    MenuItemMineWork.Visibility = Visibility.Visible;
                });
            });
        }

        private void PopupMinerGroup_Opened(object sender, EventArgs e) {
            MenuItemMinerGroup.Visibility = Visibility.Collapsed;
        }

        private void PopupMinerGroup_Closed(object sender, EventArgs e) {
            TimeSpan.FromMilliseconds(100).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    MenuItemMinerGroup.Visibility = Visibility.Visible;
                });
            });
        }

        private void MenuItemUpgrade_Click(object sender, RoutedEventArgs e) {
            var popup = PopUpgrade;
            if (popup.Child == null) {
                popup.Child = new NTMinerFileSelect(new NTMinerFileSelectViewModel(selectedResult => {
                    if (selectedResult == null || selectedResult == NTMinerFileViewModel.Empty) {
                        return;
                    }
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: selectedResult.Description, title: $"确定将选中的矿机升级到{selectedResult.Version}吗？", onYes: () => {
                        foreach (var item in Vm.SelectedMinerClients) {
                            MinerStudioRoot.MinerStudioService.UpgradeNTMinerAsync(item, selectedResult.FileName);
                        }
                    }));
                    popup.IsOpen = false;
                    MenuItemUpgrade.Visibility = Visibility.Visible;
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                        MenuItemUpgrade.Visibility = Visibility.Visible;
                    })
                });
            }
            else {
                ((NTMinerFileSelect)popup.Child).Vm.SelectedResult = null;
            }
            popup.IsOpen = true;
        }

        private void PopUpgrade_Opened(object sender, EventArgs e) {
            MenuItemUpgrade.Visibility = Visibility.Collapsed;
        }

        private void PopUpgrade_Closed(object sender, EventArgs e) {
            TimeSpan.FromMilliseconds(100).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    MenuItemUpgrade.Visibility = Visibility.Visible;
                });
            });
        }
    }
}
