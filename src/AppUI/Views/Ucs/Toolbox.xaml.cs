using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Toolbox : UserControl {
        public static string ViewId = nameof(Toolbox);

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
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
