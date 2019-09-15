using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Property : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "属性",
                IconName = "Icon_Property",
                Width = DevMode.IsDebugMode ? AppStatic.MainWindowWidth : 960,
                Height = 520,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new Property();
                return uc;
            }, fixedSize: false);
        }

        public Property() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
        }
    }
}
