using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class LocalIpConfig : UserControl {
        public static void ShowWindow(LocalIpConfigViewModel vm) {
            ContainerWindow.ShowWindow(new NTMiner.Vms.ContainerWindowViewModel {
                Title = "远程管理矿机 IP",
                IconName = "Icon_Ip",
                Width = 450,
                IsMaskTheParent = true,
                FooterVisible = Visibility.Collapsed,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new LocalIpConfig(vm);
                window.BuildCloseWindowOnecePath(uc.Vm.Id);
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e) => {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                window.BuildEventPath<GetLocalIpsResponsedEvent>("收到了获取挖矿端Ip的响应", LogEnum.DevConsole, path: message => {
                    if (message.ClientId != vm.MinerClientVm.ClientId) {
                        return;
                    }
                    vm.LocalIpVms = message.Data.Select(a => new NTMiner.Vms.LocalIpViewModel(a)).ToList();
                }, typeof(LocalIpConfig));
                MinerStudioRoot.MinerStudioService.GetLocalIpsAsync(vm.MinerClientVm);
                return uc;
            }, fixedSize: true);
        }

        public LocalIpConfigViewModel Vm {
            get; private set;
        }

        public LocalIpConfig(LocalIpConfigViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
