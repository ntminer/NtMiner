using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class VirtualMemory : UserControl {
        public static void ShowWindow(VirtualMemoryViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "远程设置虚拟内存",
                IconName = "Icon_VirtualMemory",
                CloseVisible = Visibility.Visible,
                Width = 800,
                MinWidth = 450,
                Height = 360,
                MinHeight = 360,
                IsMaskTheParent = true,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                MinerStudioRoot.MinerStudioService.GetDrivesAsync(vm.MinerClientVm);
                window.AddEventPath<GetDrivesResponsedEvent>("收到了GetDrives的响应时绑定到界面", LogEnum.DevConsole, action: message => {
                    if (message.ClientId != vm.MinerClientVm.ClientId) {
                        return;
                    }
                    vm.Drives = message.Data.Select(a => new DriveViewModel(a)).ToList();
                }, typeof(VirtualMemory));
                return new VirtualMemory(vm);
            }, fixedSize: false);
        }

        public VirtualMemoryViewModel Vm { get; private set; }

        public VirtualMemory(VirtualMemoryViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
