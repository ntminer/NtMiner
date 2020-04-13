using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class QQGroupQrCode : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "QQ群",
                IconImage = "Icon_QQ",
                Width = 280,
                Height = 320,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
            }, ucFactory: (window) => new QQGroupQrCode(), fixedSize: true);
        }

        public QQGroupQrCode() {
            InitializeComponent();
        }
    }
}
