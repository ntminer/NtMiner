using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelInputFragmentEdit : UserControl {
        public static void ShowWindow(KernelInputViewModel kernelInputVm, KernelInputFragmentViewModel fragment) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "环境变量",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                KernelInputFragmentEditViewModel vm = new KernelInputFragmentEditViewModel(kernelInputVm, fragment) {
                    CloseWindow = () => window.Close()
                };
                return new KernelInputFragmentEdit(vm);
            }, fixedSize: true);
        }

        private KernelInputFragmentEditViewModel Vm {
            get {
                return (KernelInputFragmentEditViewModel)this.DataContext;
            }
        }

        public KernelInputFragmentEdit(KernelInputFragmentEditViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
