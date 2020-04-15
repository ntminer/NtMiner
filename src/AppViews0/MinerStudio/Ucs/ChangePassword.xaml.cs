using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class ChangePassword : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "修改密码",
                IsMaskTheParent = true,
                Width = 300,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed,
                IconName = "Icon_Password"
            }, ucFactory: (window) => {
                var uc = new ChangePassword();
                window.AddCloseWindowOnecePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public ChangePasswordViewModel Vm {
            get {
                return (ChangePasswordViewModel)this.DataContext;
            }
        }

        public ChangePassword() {
            InitializeComponent();
        }
    }
}
