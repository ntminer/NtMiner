using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelSelect : UserControl, IVmFrameworkElement<CoinKernelSelectViewModel> {
        public CoinKernelSelectViewModel Vm { get; set; }

        public CoinKernelSelect(CoinKernelSelectViewModel vm) {
            this.Init(vm);
            InitializeComponent();
            this.OnLoaded(window => {
                window.BuildEventPath<CoinKernelAddedEvent>("添加了币种内核后，如果添加的币种内核是当前选中的币种的币种内核则刷新币种内核选择下拉列表的Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message => {
                    if (message.Source.CoinId == vm.Coin.Id) {
                        vm.OnPropertyChanged(nameof(vm.QueryResults));
                    }
                });
                window.BuildEventPath<CoinKernelRemovedEvent>("删除了币种内核后，如果删除的币种内核是当前选中的币种的币种内核则刷新币种内核选择下拉列表的Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message => {
                    if (message.Source.CoinId == vm.Coin.Id) {
                        vm.OnPropertyChanged(nameof(vm.QueryResults));
                    }
                });
            });
        }

        private void DataGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Vm.OnOk?.Invoke(Vm.SelectedResult);
        }

        private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                Vm.OnOk?.Invoke(Vm.SelectedResult);
                e.Handled = true;
            }
        }
    }
}
