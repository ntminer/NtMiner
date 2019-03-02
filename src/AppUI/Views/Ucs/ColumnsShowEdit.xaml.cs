using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ColumnsShowEdit : UserControl {
        public static void ShowWindow(FormType formType, ColumnsShowViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_ColumnsShow",
            }, ucFactory: (window) =>
            {
                ColumnsShowViewModel vm = new ColumnsShowViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new ColumnsShowEdit(vm);
            }, fixedSize: true);
        }

        private ColumnsShowViewModel Vm {
            get {
                return (ColumnsShowViewModel)this.DataContext;
            }
        }
        public ColumnsShowEdit(ColumnsShowViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
