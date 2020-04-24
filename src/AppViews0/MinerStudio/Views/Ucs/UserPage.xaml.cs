using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class UserPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "用户",
                IconName = "Icon_User",
                Width = 1200,
                Height = 700,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new UserPage());
        }

        public UserPageViewModel Vm { get; private set; }

        public UserPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new UserPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.AddEventPath<Per20SecondEvent>("外网群控用户列表页面打开着时周期刷新", LogEnum.DevConsole, action: message => {
                    Vm.Refresh();
                }, this.GetType());
                window.AddEventPath<UserEnabledEvent>("外网群控用户列表页面打开着时，用户启用后刷新Vm内存", LogEnum.DevConsole, action: message => {
                    UIThread.Execute(() => {
                        var userVm = Vm.QueryResults.FirstOrDefault(a => a.LoginName == message.Source.LoginName);
                        if (userVm != null) {
                            userVm.IsEnabled = true;
                        }
                    });
                }, this.GetType());
                window.AddEventPath<UserDisabledEvent>("外网群控用户列表页面打开着时，用户禁用后刷新Vm内存", LogEnum.DevConsole, action: message => {
                    UIThread.Execute(() => {
                        var userVm = Vm.QueryResults.FirstOrDefault(a => a.LoginName == message.Source.LoginName);
                        if (userVm != null) {
                            userVm.IsEnabled = false;
                        }
                    });
                }, this.GetType());
            });
        }
    }
}
