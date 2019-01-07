using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputTranslaterEdit : UserControl {
        public static void ShowEditWindow(KernelOutputTranslaterViewModel source) {
            int sortNumber = source.SortNumber;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin"
            }, ucFactory: (window) =>
            {
                KernelOutputTranslaterViewModel vm = new KernelOutputTranslaterViewModel(source.Id).Update(source);
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
