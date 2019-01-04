using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupEdit : UserControl {
        public static void ShowEditWindow(GroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Group",
                OnOk = (uc) => {
                    var vm = ((GroupEdit)uc).Vm;
                    if (NTMinerRoot.Current.GroupSet.Contains(source.Id)) {
                        Global.Execute(new UpdateGroupCommand(vm));
                    }
                    else {
                        Global.Execute(new AddGroupCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                GroupViewModel vm = new GroupViewModel(source.Id).Update(source);
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
