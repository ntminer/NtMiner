using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Property : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Property",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new Property(), fixedSize: true);
        }

        private PropertyViewModel Vm {
            get {
                return (PropertyViewModel)this.DataContext;
            }
        }

        public Property() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
