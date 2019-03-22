using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinPage : UserControl {
        public static string ViewId = nameof(CoinPage);

        public static void ShowWindow(CoinViewModel currentCoin) {
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
