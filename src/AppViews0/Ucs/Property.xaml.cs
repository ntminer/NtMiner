using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Property : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "属性",
                IconName = "Icon_Property",
                Width = 1000,
                Height = 420,
                CloseVisible = Visibility.Visible,
                IsChildWindow = true,
                IsMaskTheParent = false
            }, ucFactory: (window) => {
                var uc = new Property();
                return uc;
            }, fixedSize: false);
        }

        public Property() {
            InitializeComponent();
        }
    }
}
