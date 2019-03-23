using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolEdit : UserControl {
        public static string ViewId = nameof(PoolEdit);

        public static void ShowWindow(FormType formType, PoolViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IconName = "Icon_Pool",
                IsDialogWindow = true,
                Width = 580,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                PoolViewModel vm = new PoolViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new PoolEdit(vm);
            }, fixedSize: true);
        }

        private PoolViewModel Vm {
            get {
                return (PoolViewModel)this.DataContext;
            }
        }

        public PoolEdit(PoolViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<PoolKernelViewModel>(sender, e);
        }
    }
}
