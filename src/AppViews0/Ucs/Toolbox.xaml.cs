using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Toolbox : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "工具箱",
                IconName = "Icon_Toolbox",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new Toolbox(), fixedSize: true);
        }

        private ToolboxViewModel Vm {
            get {
                return (ToolboxViewModel)this.DataContext;
            }
        }

        public Toolbox() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
            this.RunOneceOnLoaded((window) => {
                window.Activated += (object sender, EventArgs e) => {
                    Vm.OnPropertyChanged(nameof(Vm.IsAutoAdminLogon));
                    Vm.OnPropertyChanged(nameof(Vm.AutoAdminLogonMessage));
                    Vm.OnPropertyChanged(nameof(Vm.IsRemoteDesktopEnabled));
                    Vm.OnPropertyChanged(nameof(Vm.RemoteDesktopMessage));
                };
                window.On<RegCmdHereEvent>("执行添加windows右键命令行命令后通过弹窗反馈命令执行结果", LogEnum.None,
                    action: message => {
                        if (message.IsSuccess) {
                            NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage(message.Message);
                        }
                        else {
                            NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage(message.Message);
                        }
                    });
                window.On<BlockWAUEvent>("执行禁用windows系统更新命令后通过弹窗反馈命令执行结果", LogEnum.None,
                    action: message => {
                        if (message.IsSuccess) {
                            NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage(message.Message);
                        }
                        else {
                            NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage(message.Message);
                        }
                    });
                window.On<Win10OptimizeEvent>("执行优化windows命令后通过弹窗反馈命令执行结果", LogEnum.None,
                    action: message => {
                        if (message.IsSuccess) {
                            NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage(message.Message);
                        }
                        else {
                            NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage(message.Message);
                        }
                    });
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
