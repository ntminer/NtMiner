using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupEdit : UserControl {
        public static void ShowEditWindow(GroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Group"
            }, ucFactory: (window) =>
            {
                GroupViewModel vm = new GroupViewModel(source.Id).Update(source);
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
