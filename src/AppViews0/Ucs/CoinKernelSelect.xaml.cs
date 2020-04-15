using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelSelect : UserControl {
        public CoinKernelSelectViewModel Vm {
            get {
                return (CoinKernelSelectViewModel)this.DataContext;
            }
        }

        public CoinKernelSelect(CoinKernelSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.AddEventPath<CoinKernelVmAddedEvent>("添加了币种内核后，如果添加的币种内核是当前选中的币种的币种内核则刷新币种内核选择下拉列表的Vm内存", LogEnum.DevConsole, action: message => {
                    if (message.Event.Source.CoinId == vm.Coin.Id) {
                        vm.OnPropertyChanged(nameof(vm.QueryResults));
                    }
                }, this.GetType());
                window.AddEventPath<CoinKernelVmRemovedEvent>("删除了币种内核后，如果删除的币种内核是当前选中的币种的币种内核则刷新币种内核选择下拉列表的Vm内存", LogEnum.DevConsole, action: message => {
                    if (message.Event.Source.CoinId == vm.Coin.Id) {
                        vm.OnPropertyChanged(nameof(vm.QueryResults));
                    }
                }, this.GetType());
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
