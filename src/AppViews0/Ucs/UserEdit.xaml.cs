using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class UserEdit : UserControl {
        public static void ShowWindow(FormType formType, UserViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "用户",
                FormType = formType,
                Width = 520,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_User"
            }, ucFactory: (window) => {
                UserViewModel vm = new UserViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(UserEdit));
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
