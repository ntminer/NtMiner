using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核",
                FormType = formType,
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                Width = 620,
                Height = 400,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                KernelViewModel vm = new KernelViewModel(source) {
                    CloseWindow = () => window.Close()
                };
                return new KernelEdit(vm);
            }, fixedSize: false);
        }

        private KernelViewModel Vm {
            get {
                return (KernelViewModel)this.DataContext;
            }
        }

        public KernelEdit(KernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.CbCoins.SelectedItem = CoinViewModel.PleaseSelect;
        }

        private void CoinKernelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<CoinKernelViewModel>(sender, e);
        }

        private void KbButtonBrand_Clicked(object sender, RoutedEventArgs e) {
            OpenBrandPopup();
            e.Handled = true;
        }

        private void OpenBrandPopup() {
            var popup = PopupBrand;
            popup.IsOpen = true;
            var selected = Vm.BrandItem;
            popup.Child = new SysDicItemSelect(
                new SysDicItemSelectViewModel(AppContext.Instance.SysDicItemVms.KernelBrandItems, selected, onOk: selectedResult => {
                    if (selectedResult != null) {
                        if (Vm.BrandItem != selectedResult) {
                            Vm.BrandItem = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        popup.IsOpen = false;
                    })
                });
        }
    }
}
