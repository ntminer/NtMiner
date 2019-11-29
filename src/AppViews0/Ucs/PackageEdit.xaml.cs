using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PackageEdit : UserControl {
        public static void ShowWindow(FormType formType, PackageViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核包",
                FormType = formType,
                IconName = "Icon_Package",
                IsMaskTheParent = true,
                Width = 860,
                Height = 300,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                PackageViewModel vm = new PackageViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(PackageEdit));
                return new PackageEdit(vm);
            }, fixedSize: false);
        }

        private PackageViewModel Vm {
            get {
                return (PackageViewModel)this.DataContext;
            }
        }

        public PackageEdit(PackageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
