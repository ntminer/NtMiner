using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class UserEdit : UserControl {
        public static void ShowWindow(FormType formType, UserViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_User"
            }, ucFactory: (window) => {
                UserViewModel vm = new UserViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new UserEdit(vm);
            }, fixedSize: true);
        }

        private UserViewModel Vm {
            get {
                return (UserViewModel)this.DataContext;
            }
        }

        public UserEdit(UserViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
