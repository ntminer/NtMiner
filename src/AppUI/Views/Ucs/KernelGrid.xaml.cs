using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelGrid : UserControl {
        public static string ViewId = nameof(KernelGrid);

        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        private readonly List<IDelegateHandler> _handlers = new List<IDelegateHandler>();
        public KernelGrid() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            VirtualRoot.On<CoinKernelAddedEvent>(
                 "添加了币种内核后刷新VM内存",
                 LogEnum.Console,
                 action: (message) => {
                     Vm.OnPropertyChanged(nameof(Vm.QueryResults));
                 }).AddToCollection(_handlers);
            VirtualRoot.On<CoinKernelUpdatedEvent>(
                "更新了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.OnPropertyChanged(nameof(Vm.QueryResults));
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinKernelRemovedEvent>(
                "移除了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.OnPropertyChanged(nameof(Vm.QueryResults));
                }).AddToCollection(_handlers);
            this.Unloaded += (object sender, System.Windows.RoutedEventArgs e) => {
                foreach (var handler in _handlers) {
                    VirtualRoot.UnPath(handler);
                }
            };
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((KernelViewModel)dg.SelectedItem).Edit.Execute(FormType.Edit);
            }
        }

        private void DataGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Vm.KernelDownloadingVisible = System.Windows.Visibility.Collapsed;
        }

        private void ButtonLeftCoin_Click(object sender, System.Windows.RoutedEventArgs e) {
            ButtonLeft.IsEnabled = Vm.PageNumber != 1;
            ButtonRight.IsEnabled = Vm.PageNumber != Vm.PageNumbers.Count;
        }

        private void ButtonRightCoin_Click(object sender, System.Windows.RoutedEventArgs e) {
            ButtonLeft.IsEnabled = Vm.PageNumber != 1;
            ButtonRight.IsEnabled = Vm.PageNumber != Vm.PageNumbers.Count;
        }
    }
}
