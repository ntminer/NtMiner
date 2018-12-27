using MahApps.Metro.Controls;
using NTMiner.Notifications;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MainWindow : MetroWindow, IMainWindow {
        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
            this.StateChanged += (s, e) => {
                if (Vm.MinerProfile.IsShowInTaskbar) {
                    ShowInTaskbar = true;
                }
                else {
                    if (WindowState == WindowState.Minimized) {
                        ShowInTaskbar = false;
                    }
                    else {
                        ShowInTaskbar = true;
                    }
                }
            };
            InitializeComponent();
            if (!Windows.Role.IsAdministrator) {
                Vm.Manager
                    .CreateMessage()
                    .Warning("请以管理员身份运行。")
                    .WithButton("点击以管理员身份运行", button => {
                        AppStatic.RunAsAdministrator.Execute(null);
                    })
                    .Dismiss().WithButton("忽略", button => {
                        Vm.IsBtnRunAsAdministratorVisible = Visibility.Visible;
                    }).Queue();
            }
            if (NTMinerRoot.Current.GpuSet.Count == 0) {
                Vm.Manager
                    .CreateMessage()
                    .Warning("没有检测到矿卡。")
                    .Dismiss().WithButton("忽略", button => {

                    }).Queue();
            }
        }
        
        public void ShowThisWindow() {
            ShowInTaskbar = true;
            if (WindowState == WindowState.Minimized) {
                WindowState = WindowState.Normal;
            }
            this.Activate();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
