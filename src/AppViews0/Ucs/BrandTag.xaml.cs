using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class BrandTag : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "生成品牌专版（在品牌上右键）",
                IsMaskTheParent = true,
                Width = 800,
                CloseVisible = Visibility.Visible,
                IconName = "Icon_Coin",
            }, ucFactory: (window) =>
            {
                return new BrandTag();
            }, fixedSize: false);
        }

        private BrandTagViewModel Vm {
            get {
                return (BrandTagViewModel)this.DataContext;
            }
        }
        public BrandTag() {
            InitializeComponent();
        }
    }
}
