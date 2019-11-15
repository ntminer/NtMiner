using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupEdit : UserControl {
        public static void ShowWindow(FormType formType, GroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "组",
                FormType = formType,
                IsDialogWindow = true,
                Width = 380,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Group"
            }, ucFactory: (window) =>
            {
                GroupViewModel vm = new GroupViewModel(source) {
                    CloseWindow = window.Close
                };
                return new GroupEdit(vm);
            }, fixedSize: true);
        }

        private GroupViewModel Vm {
            get {
                return (GroupViewModel)this.DataContext;
            }
        }

        public GroupEdit(GroupViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
