using NTMiner.MinerStudio.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerClient : UserControl {
        public MinerClientViewModel Vm {
            get {
                return (MinerClientViewModel)this.DataContext;
            }
        }

        public MinerClient() {
            InitializeComponent();
            this.TbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.OnLoaded((window) => {
                window.BuildEventPath<MinutePartChangedEvent>("整分钟时更新MinerClient上的日期时间计时器", LogEnum.None,
                    path: message => {
                        this.TbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    }, location: this.GetType());
            });
        }

        private void TbIp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MinerClientViewModel vm = (MinerClientViewModel)((FrameworkElement)sender).Tag;
            vm.RemoteDesktop.Execute(vm.GetRemoteDesktopIp());
            e.Handled = true;
        }
    }
}
