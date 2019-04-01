using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class OuterProperty : UserControl {
        public static string ViewId = nameof(OuterProperty);

        public static void ShowWindow() {
            ContainerWindow.ShowWindow("属性", new ContainerWindowViewModel {
                IconName = "Icon_Property",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new OuterProperty(), fixedSize: true);
        }

        private OuterPropertyViewModel Vm {
            get {
                return (OuterPropertyViewModel)this.DataContext;
            }
        }

        public OuterProperty() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
        }
    }
}
