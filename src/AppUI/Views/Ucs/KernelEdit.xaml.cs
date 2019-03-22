using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelEdit : UserControl {
        public static string ViewId = nameof(KernelEdit);

        public static void ShowWindow(FormType formType, KernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                Width = 620,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                KernelViewModel vm = new KernelViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new KernelEdit(vm);
            }, fixedSize: false);
        }

        private KernelViewModel Vm {
            get {
                return (KernelViewModel)this.DataContext;
            }
        }

        private readonly List<IDelegateHandler> _handlers = new List<IDelegateHandler>();
        public KernelEdit(KernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.CbCoins.SelectedItem = CoinViewModel.PleaseSelect;
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources); VirtualRoot.On<CoinKernelAddedEvent>(
                 "添加了币种内核后刷新VM内存",
                 LogEnum.Console,
                 action: (message) => {
                     Vm.OnPropertyChanged(nameof(Vm.CoinKernels));
                     Vm.OnPropertyChanged(nameof(Vm.CoinVms));
                 }).AddToCollection(_handlers);
            VirtualRoot.On<CoinKernelUpdatedEvent>(
                "更新了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.OnPropertyChanged(nameof(Vm.CoinKernels));
                    Vm.OnPropertyChanged(nameof(Vm.CoinVms));
                }).AddToCollection(_handlers);
            VirtualRoot.On<CoinKernelRemovedEvent>(
                "移除了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    Vm.OnPropertyChanged(nameof(Vm.CoinKernels));
                    Vm.OnPropertyChanged(nameof(Vm.CoinVms));
                }).AddToCollection(_handlers);
            this.Unloaded += (object sender, System.Windows.RoutedEventArgs e) => {
                foreach (var handler in _handlers) {
                    VirtualRoot.UnPath(handler);
                }
            };
        }

        private void CoinKernelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<CoinKernelViewModel>(sender, e);
        }
    }
}
