using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinPage : UserControl {
        public static string ViewId = nameof(CoinPage);

        public static void ShowWindow(CoinViewModel currentCoin, string tabName) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Coin",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = DevMode.IsDebugMode ? 960 : 860,
                Height = 520
            }, 
            ucFactory: (window) => new CoinPage(),
            beforeShow: uc => {
                if (currentCoin != null) {
                    CoinPageViewModel vm = (CoinPageViewModel)uc.DataContext;
                    switch (tabName) {
                        case "wallet":
                            vm.IsWalletTabSelected = true;
                            break;
                        case "kernel":
                            vm.IsKernelTabSelected = true;
                            break;
                        case "pool":
                        default:
                            vm.IsPoolTabSelected = true;
                            break;
                    }
                    vm.CurrentCoin = currentCoin;
                }
            });
        }

        private CoinPageViewModel Vm {
            get {
                return (CoinPageViewModel)this.DataContext;
            }
        }

        private readonly List<IDelegateHandler> _handlers = new List<IDelegateHandler>();
        public CoinPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            VirtualRoot.On<WalletAddedEvent>(
                "添加了钱包后刷新VM",
                LogEnum.Console,
                action: message => {
                    if (message.Source.CoinId == Vm.CurrentCoin?.Id) {
                        Vm.OnPropertyChanged(nameof(Vm.CurrentCoin));
                    }
                }).AddToCollection(_handlers);
            VirtualRoot.On<WalletRemovedEvent>(
                "添加了钱包后刷新VM",
                LogEnum.Console,
                action: message => {
                    if (message.Source.CoinId == Vm.CurrentCoin?.Id) {
                        Vm.OnPropertyChanged(nameof(Vm.CurrentCoin));
                    }
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinAddedEvent>(
                 "添加了币种后刷新VM内存",
                 LogEnum.Console,
                 action: (message) => {
                     Vm.OnPropertyChanged(nameof(Vm.List));
                     Vm.OnPropertyChanged(nameof(Vm.CurrentCoin));
                 }).AddToCollection(_handlers);
            VirtualRoot.On<CoinRemovedEvent>(
                "移除了币种后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    Vm.OnPropertyChanged(nameof(Vm.List));
                    Vm.OnPropertyChanged(nameof(Vm.CurrentCoin));
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinUpdatedEvent>(
                "更新了币种后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    Vm.OnPropertyChanged(nameof(Vm.List));
                    Vm.OnPropertyChanged(nameof(Vm.CurrentCoin));
                }).AddToCollection(_handlers);

            VirtualRoot.On<CoinKernelAddedEvent>(
                "添加了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.CurrentCoin?.OnPropertyChanged(nameof(Vm.CurrentCoin.CoinKernels));
                    Vm.CurrentCoin?.OnPropertyChanged(nameof(Vm.CurrentCoin.CoinKernel));
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinKernelUpdatedEvent>(
                "更新了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.CurrentCoin?.OnPropertyChanged(nameof(Vm.CurrentCoin.CoinKernels));
                    Vm.CurrentCoin?.OnPropertyChanged(nameof(Vm.CurrentCoin.CoinKernel));
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinKernelRemovedEvent>(
                "移除了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.CurrentCoin?.OnPropertyChanged(nameof(Vm.CurrentCoin.CoinKernels));
                    Vm.CurrentCoin?.OnPropertyChanged(nameof(Vm.CurrentCoin.CoinKernel));
                }).AddToCollection(_handlers);
            this.Unloaded += (object sender, System.Windows.RoutedEventArgs e) => {
                foreach (var handler in _handlers) {
                    VirtualRoot.UnPath(handler);
                }
            };
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<CoinViewModel>(sender, e);
        }

        private void WalletDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<WalletViewModel>(sender, e);
        }

        private void PoolDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<PoolViewModel>(sender, e);
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<CoinKernelViewModel>(sender, e);
        }
    }
}
