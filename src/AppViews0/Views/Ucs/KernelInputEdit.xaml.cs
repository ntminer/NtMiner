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
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                KernelInputViewModel vm = new KernelInputViewModel(source);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new KernelInputEdit(vm);
            }, fixedSize: true);
        }

        public KernelInputViewModel Vm { get; private set; }

        public KernelInputEdit(KernelInputViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
