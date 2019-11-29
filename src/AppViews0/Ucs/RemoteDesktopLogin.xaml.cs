using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class RemoteDesktopLogin : UserControl {
        public static void ShowWindow(RemoteDesktopLoginViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "登录远程桌面 - " + vm.Ip,
                IconName = "Icon_RemoteDesktop",
                Width = 400,
                Height = 160,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => {
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(RemoteDesktopLogin));
                return new RemoteDesktopLogin(vm);
            }, fixedSize: true);
        }

        public RemoteDesktopLogin(RemoteDesktopLoginViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
