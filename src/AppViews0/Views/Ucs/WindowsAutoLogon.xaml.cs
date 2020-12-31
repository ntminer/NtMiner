using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WindowsAutoLogon : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Windows 2004或更高版本的Windows",
                IconName = "Icon_User",
                Width = 380,
                Height = 180,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                IsMaskTheParent = true,
                IsChildWindow = true
            }, ucFactory: (window) => {
                WindowsAutoLogonViewModel vm = new WindowsAutoLogonViewModel();
                window.BuildCloseWindowOnecePath(vm.Id);
                return new WindowsAutoLogon(vm);
            }, beforeShow: (window, uc)=> {
                uc.PasswordFocus();
            }, fixedSize: true);
        }

        public WindowsAutoLogonViewModel Vm { get; private set; }

        public WindowsAutoLogon(WindowsAutoLogonViewModel vm) {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = vm;
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        public void PasswordFocus() {
            if (string.IsNullOrEmpty(this.TbLoginName.Text)) {
                this.TbLoginName.Focus();
            }
            else {
                this.PbPassword.Focus();
            }
        }
    }
}
