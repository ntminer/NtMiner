using NTMiner.MinerStudio;
using NTMiner.MinerStudio.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerClientsSearchBar : UserControl {
        public MinerClientsWindowViewModel Vm {
            get {
                return MinerStudioRoot.MinerClientsWindowVm;
            }
        }

        public MinerClientsSearchBar() {
            this.DataContext = Vm;
            InitializeComponent();
        }

        private void BtnPoolPop_Click(object sender, RoutedEventArgs e) {
            PopupPool.IsOpen = true;
            e.Handled = true;
        }

        private void KbButtonMineWork_Clicked(object sender, RoutedEventArgs e) {
            var popup = PopupMineWork;
            var selected = Vm.SelectedMineWork;
            if (popup.Child == null) {
                popup.Child = new MineWorkSelect(new MineWorkSelectViewModel("根据作业筛选矿机", selected, selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.SelectedMineWork != selectedResult) {
                            Vm.SelectedMineWork = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
            }
            else {
                ((MineWorkSelect)popup.Child).Vm.SelectedResult = selected;
            }
            popup.IsOpen = true;
        }

        private void LbMineWork_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            PopupMineWork.IsOpen = false;
        }

        private void CollapsePopupMineWork(object sender, RoutedEventArgs e) {
            PopupMineWork.IsOpen = false;
        }

        private void LbMinerGroup_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            PopupMinerGroup.IsOpen = false;
        }

        private void CollapsePopupMinerGroup(object sender, RoutedEventArgs e) {
            PopupMinerGroup.IsOpen = false;
        }

        private void KbButtonMinerGroup_Clicked(object sender, RoutedEventArgs e) {
            var popup = PopupMinerGroup;
            var selected = Vm.SelectedMinerGroup;
            if (popup.Child == null) {
                popup.Child = new MinerGroupSelect(new MinerGroupSelectViewModel("根据分组筛选矿机", selected, selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.SelectedMinerGroup != selectedResult) {
                            Vm.SelectedMinerGroup = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
            }
            else {
                ((MinerGroupSelect)popup.Child).Vm.SelectedResult = selected;
            }
            popup.IsOpen = true;
        }

        private void LbPool_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            PopupPool.IsOpen = false;
        }
    }
}
