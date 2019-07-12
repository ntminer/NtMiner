using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class Calc : UserControl {
        public static void ShowWindow(CoinViewModel coin) {
            if (coin == null) {
                coin = AppContext.Instance.MinerProfileVm.CoinVm;
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "收益计算器",
                IconName = "Icon_Calc",
                Width = 760,
                MinWidth = 760,
                Height = 600,
                CloseVisible = Visibility.Visible,
                FooterText = "数据来自鱼池首页，感谢鱼池的支持。因为数据来自矿池，单位算力收益的币数是非常准确的。"
            }, ucFactory: (window) => {
                var uc = new Calc();
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e) => {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                return uc;
            }, fixedSize: false);
        }

        public CalcViewModel Vm {
            get {
                return (CalcViewModel)this.DataContext;
            }
        }

        private Calc() {
            InitializeComponent();
            this.RunOneceOnLoaded(() => {
                var window = Window.GetWindow(this);
                window.On<CalcConfigSetInitedEvent>("收益计算器数据集刷新后刷新VM", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            foreach (var coinVm in Vm.CoinVms.AllCoins) {
                                coinVm.CoinIncomeVm.Refresh();
                            }
                        });
                    });
                window.On<Per1MinuteEvent>("当收益计算器页面打开着的时候周期刷新", LogEnum.None,
                    action: message => {
                        if (Vm.CoinVms.AllCoins.Count == 0) {
                            return;
                        }
                        if (Vm.CoinVms.AllCoins.Max(a => a.CoinIncomeVm.ModifiedOn).AddMinutes(10) < DateTime.Now) {
                            NTMinerRoot.Instance.CalcConfigSet.Init(forceRefresh: true);
                        }
                    });
                foreach (var coinVm in Vm.CoinVms.AllCoins) {
                    coinVm.CoinIncomeVm.Refresh();
                }
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
