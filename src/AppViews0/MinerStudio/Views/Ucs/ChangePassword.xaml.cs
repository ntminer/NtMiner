using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
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
            }, beforeShow: (window, uc)=> {
                uc.DoFocus();
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

        private void DoFocus() {
            this.TbOldPassword.Focus();
        }
    }
}
