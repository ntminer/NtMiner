using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupEdit : UserControl {
        public static void ShowWindow(FormType formType, GroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "组",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 380,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Group"
            }, ucFactory: (window) =>
            {
                GroupViewModel vm = new GroupViewModel(source);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new GroupEdit(vm);
            }, fixedSize: true);
        }

        public GroupViewModel Vm { get; private set; }

        public GroupEdit(GroupViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
