using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelInputEdit : UserControl {
        public static string ViewId = nameof(KernelInputEdit);

        public static void ShowWindow(FormType formType, KernelInputViewModel source) {
            ContainerWindow.ShowWindow("内核输入", new ContainerWindowViewModel {
                FormType = formType,
                IconName = "Icon_KernelInput",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                KernelInputViewModel vm = new KernelInputViewModel(source) {
                    CloseWindow = () => window.Close()
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
