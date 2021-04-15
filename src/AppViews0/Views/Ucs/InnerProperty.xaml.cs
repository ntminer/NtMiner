using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InnerProperty : UserControl {
        public InnerPropertyViewModel Vm { get; private set; }

        public InnerProperty() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new InnerPropertyViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                window.BuildEventPath<ServerJsonVersionChangedEvent>("刷新展示的ServerJsonVersion", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal, path: message => {
                    if (message.IsChagned) {
                        Vm.ServerJsonVersion = message.NewVersion;
                    }
                });
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
