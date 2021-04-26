using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace NTMiner.Views.Ucs {
    public partial class Toolbox : UserControl {
        public ToolboxViewModel Vm { get; private set; }

        public Toolbox() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new ToolboxViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.OnLoaded(window => {
                window.Activated += (object sender, EventArgs e) => {
                    Vm.OnPropertyChanged(nameof(Vm.IsAutoAdminLogon));
                    Vm.OnPropertyChanged(nameof(Vm.AutoAdminLogonMessage));
                    Vm.OnPropertyChanged(nameof(Vm.IsRemoteDesktopEnabled));
                    Vm.OnPropertyChanged(nameof(Vm.RemoteDesktopMessage));
                };
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void BtnMinerClientFinder_Click(object sender, RoutedEventArgs e) {
            if (this.MinerClientFinderIcon.Visibility == Visibility.Visible) {
                Process process = Process.GetProcessesByName(NTKeyword.MinerClientFinderProcessName).FirstOrDefault();
                if (process == null) {
                    this.MinerClientFinderIcon.Visibility = Visibility.Collapsed;
                    this.MinerClientFinderLoadingIcon.Visibility = Visibility.Visible;
                    // 这里的逻辑是每100毫秒检查一次MinerClientFinder进程是否存在，每检查一次将loading图标
                    // 旋转30度，如果MinerClientFinder进程存在了或者已经检查了3秒钟了则停止检查。
                    VirtualRoot.SetInterval(
                        per: TimeSpan.FromMilliseconds(100),
                        perCallback: () => {
                            UIThread.Execute(() => {
                                ((RotateTransform)this.MinerClientFinderLoadingIcon.RenderTransform).Angle += 30;
                            });
                        },
                        stopCallback: () => {
                            UIThread.Execute(() => {
                                this.MinerClientFinderIcon.Visibility = Visibility.Visible;
                                this.MinerClientFinderLoadingIcon.Visibility = Visibility.Collapsed;
                                ((RotateTransform)this.MinerClientFinderLoadingIcon.RenderTransform).Angle = 0;
                            });
                        },
                        timeout: TimeSpan.FromSeconds(3),
                        requestStop: () => {
                            return Process.GetProcessesByName(NTKeyword.MinerClientFinderProcessName).FirstOrDefault() != null;
                        }
                    );
                }
            }
        }
    }
}
