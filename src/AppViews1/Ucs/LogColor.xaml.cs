using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class LogColor : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "日志配色",
                IconName = "Icon_Theme",
                CloseVisible = System.Windows.Visibility.Visible,
            }, ucFactory: (window) => new LogColor(), fixedSize: true);
        }

        public LogColorViewModel Vm {
            get {
                return (LogColorViewModel)this.DataContext;
            }
        }

        public LogColor() {
            InitializeComponent();
        }
    }
}
