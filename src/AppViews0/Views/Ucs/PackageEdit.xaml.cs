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
                window.AddCloseWindowOnecePath(vm.Id);
                return new PackageEdit(vm);
            }, fixedSize: false);
        }

        public PackageViewModel Vm { get; private set; }

        public PackageEdit(PackageViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
