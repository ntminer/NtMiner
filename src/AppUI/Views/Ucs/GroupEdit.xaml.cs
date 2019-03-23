using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupEdit : UserControl {
        public static string ViewId = nameof(GroupEdit);

        public static void ShowWindow(FormType formType, GroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Group"
            }, ucFactory: (window) =>
            {
                GroupViewModel vm = new GroupViewModel(source);
                vm.CloseWindow = () => window.Close();
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
