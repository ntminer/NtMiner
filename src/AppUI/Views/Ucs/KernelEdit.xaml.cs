using NTMiner.Vms;
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

        public KernelEdit(KernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.CbCoins.SelectedItem = CoinViewModel.PleaseSelect;
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void CoinKernelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<CoinKernelViewModel>(sender, e);
        }
    }
}
