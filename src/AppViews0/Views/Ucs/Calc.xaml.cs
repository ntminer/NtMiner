using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Calc : UserControl {
        public static void ShowWindow(CoinViewModel coin) {
            if (coin == null) {
                coin = AppRoot.MinerProfileVm.CoinVm;
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "收益计算器",
                IconName = "Icon_Calc",
                Width = 960,
                Height = 560,
                CloseVisible = Visibility.Visible,
                FooterText = "数据来自鱼池首页，感谢鱼池的支持。因为数据来自矿池，单位算力收益的币数是非常准确的。"
            }, ucFactory: (window) => {
                var uc = new Calc();
                return uc;
            }, fixedSize: false);
        }

        public CalcViewModel Vm { get; private set; }

        private Calc() {
            this.Vm = new CalcViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded((window) => {
                window.AddEventPath<CalcConfigSetInitedEvent>("收益计算器数据集刷新后刷新VM", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => () => {
                            foreach (var coinVm in Vm.CoinVms.AllCoins) {
                                coinVm.CoinIncomeVm.Refresh();
                            }
                        });
                    }, location: this.GetType());
                window.AddEventPath<Per1MinuteEvent>("当收益计算器页面打开着的时候周期刷新", LogEnum.None,
                    action: message => {
                        if (Vm.CoinVms.AllCoins.Count == 0) {
                            return;
                        }
                        if (Vm.CoinVms.AllCoins.Max(a => a.CoinIncomeVm.ModifiedOn).AddMinutes(10) < DateTime.Now) {
                            NTMinerContext.Instance.CalcConfigSet.Init(forceRefresh: true);
                        }
                    }, location: this.GetType());
                NTMinerContext.Instance.CalcConfigSet.Init(forceRefresh: true);
            });
        }

        private void UnitButton_Click(object sender, RoutedEventArgs e) {
            var fe = (FrameworkElement)sender;
            PopupMain.PlacementTarget = fe;
            PopupMain.IsOpen = true;
            _dump = true;
            var speedUnitVm = ((CoinIncomeViewModel)fe.Tag).SpeedUnitVm;
            Vm.SpeedUnitVm = speedUnitVm;
            _dump = false;
            e.Handled = true;
        }

        private bool _dump = false;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var fe = (FrameworkElement)PopupMain.PlacementTarget;
            if (fe != null && PopupMain.IsOpen) {
                ((CoinIncomeViewModel)fe.Tag).SpeedUnitVm = Vm.SpeedUnitVm;
            }
            if (!_dump) {
                PopupMain.IsOpen = false;
            }
        }

        private void PopupMain_Opened(object sender, EventArgs e) {
            var fe = (FrameworkElement)PopupMain.PlacementTarget;
            if (fe != null) {
                fe.IsEnabled = !PopupMain.IsOpen;
            }
        }

        private void PopupMain_Closed(object sender, EventArgs e) {
            var fe = (FrameworkElement)PopupMain.PlacementTarget;
            if (fe != null) {
                fe.IsEnabled = !PopupMain.IsOpen;
            }
        }
    }
}
