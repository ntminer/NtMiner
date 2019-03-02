using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputFilterEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputFilterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin"
            }, ucFactory: (window) =>
            {
                KernelOutputFilterViewModel vm = new KernelOutputFilterViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new KernelOutputFilterEdit(vm);
            }, fixedSize: true);
        }

        private KernelOutputFilterViewModel Vm {
            get {
                return (KernelOutputFilterViewModel)this.DataContext;
            }
        }
        public KernelOutputFilterEdit(KernelOutputFilterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
