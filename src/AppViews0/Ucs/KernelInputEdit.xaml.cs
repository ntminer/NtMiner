using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelInputEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelInputViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输入",
                FormType = formType,
                IconName = "Icon_KernelInput",
                Width = 660,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                KernelInputViewModel vm = new KernelInputViewModel(source) {
                    CloseWindow = window.Close
                };
                return new KernelInputEdit(vm);
            }, fixedSize: true);
        }

        private KernelInputViewModel Vm {
            get {
                return (KernelInputViewModel)this.DataContext;
            }
        }

        public KernelInputEdit(KernelInputViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
