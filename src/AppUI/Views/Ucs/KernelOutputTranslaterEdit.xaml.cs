using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputTranslaterEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputTranslaterViewModel source) {
            int sortNumber = source.SortNumber;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin"
            }, ucFactory: (window) =>
            {
                KernelOutputTranslaterViewModel vm = new KernelOutputTranslaterViewModel(source);
                vm.CloseWindow = () => window.Close();
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
