using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class UserEdit : UserControl {
        public static void ShowWindow(FormType formType, UserViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "用户",
                FormType = formType,
                Width = 520,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_User"
            }, ucFactory: (window) => {
                UserViewModel vm = new UserViewModel(source) {
                    CloseWindow = window.Close
                };
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
        }
    }
}
