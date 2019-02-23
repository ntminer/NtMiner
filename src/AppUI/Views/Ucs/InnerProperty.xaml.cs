using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InnerProperty : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Property",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new InnerProperty(), fixedSize: true);
        }

        private InnerPropertyViewModel Vm {
            get {
                return (InnerPropertyViewModel)this.DataContext;
            }
        }

        public InnerProperty() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
