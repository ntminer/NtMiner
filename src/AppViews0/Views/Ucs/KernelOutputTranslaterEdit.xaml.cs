using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputTranslaterEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputTranslaterViewModel source) {
            int sortNumber = source.SortNumber;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输出翻译器",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 520,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin"
            }, ucFactory: (window) =>
            {
                KernelOutputTranslaterViewModel vm = new KernelOutputTranslaterViewModel(source);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new KernelOutputTranslaterEdit(vm);
            }, fixedSize: true);
        }

        public KernelOutputTranslaterViewModel Vm { get; private set; }

        public KernelOutputTranslaterEdit(KernelOutputTranslaterViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
