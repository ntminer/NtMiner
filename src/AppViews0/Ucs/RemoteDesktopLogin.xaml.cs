using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class RemoteDesktopLogin : UserControl {
        public static void ShowWindow(RemoteDesktopLoginViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "登录远程桌面",
                IconName = "Icon_RemoteDesktop",
                Width = 360,
                Height = 150,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => {
                vm.CloseWindow = window.Close;
                return new RemoteDesktopLogin(vm);
            }, fixedSize: true);
        }

        public RemoteDesktopLogin(RemoteDesktopLoginViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
