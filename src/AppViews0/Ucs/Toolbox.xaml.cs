using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Toolbox : UserControl {
        private ToolboxViewModel Vm {
            get {
                return (ToolboxViewModel)this.DataContext;
            }
        }

        public Toolbox() {
            InitializeComponent();
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.RunOneceOnLoaded((window) => {
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
    }
}
