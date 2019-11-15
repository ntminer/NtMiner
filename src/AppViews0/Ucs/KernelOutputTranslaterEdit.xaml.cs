using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputTranslaterEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputTranslaterViewModel source) {
            int sortNumber = source.SortNumber;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输出翻译器",
                FormType = formType,
                IsDialogWindow = true,
                Width = 520,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin"
            }, ucFactory: (window) =>
            {
                KernelOutputTranslaterViewModel vm = new KernelOutputTranslaterViewModel(source) {
                    CloseWindow = window.Close
                };
                return new KernelOutputTranslaterEdit(vm);
            }, fixedSize: true);
        }

        private KernelOutputTranslaterViewModel Vm {
            get {
                return (KernelOutputTranslaterViewModel)this.DataContext;
            }
        }
        public KernelOutputTranslaterEdit(KernelOutputTranslaterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
