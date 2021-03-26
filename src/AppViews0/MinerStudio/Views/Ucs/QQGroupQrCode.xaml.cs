using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
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

        public QQGroupQrCodeViewModel Vm { get; private set; }

        public QQGroupQrCode() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new QQGroupQrCodeViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }
    }
}
