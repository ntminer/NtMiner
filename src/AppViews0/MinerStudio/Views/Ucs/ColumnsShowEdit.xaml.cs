using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class ColumnsShowEdit : UserControl {
        public static void ShowWindow(ColumnsShowViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "定制列分组",
                FormType = FormType.Edit,
                IsMaskTheParent = false,
                IsChildWindow = true,
                Width = 850,
                Height = 340,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_ColumnsShow"
            }, ucFactory: (window) => {
                // 这里没有深拷贝传入的vm，因为在矿机DataGrid的列头上也可以隐藏列，所以两个地方操作的需要是同一个vm。
                window.AddCloseWindowOnecePath(vm.Id);
                return new ColumnsShowEdit(vm);
            }, fixedSize: false);
        }

        public ColumnsShowViewModel Vm {
            get {
                return (ColumnsShowViewModel)this.DataContext;
            }
        }

        public ColumnsShowEdit(ColumnsShowViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
