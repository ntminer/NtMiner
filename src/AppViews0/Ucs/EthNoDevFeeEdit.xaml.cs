using NTMiner.Vms;
using System.Diagnostics;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class EthNoDevFeeEdit : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_NoDevFee",
                Title = "配置Claymore ETH反抽水",
                Width = 600,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new EthNoDevFeeEdit();
                uc.Vm.CloseWindow = window.Close;
                return uc;
            }, fixedSize: true);
        }

        private EthNoDevFeeEditViewModel Vm {
            get {
                return (EthNoDevFeeEditViewModel)this.DataContext;
            }
        }

        public EthNoDevFeeEdit() {
            InitializeComponent();
        }

        private void Help_Click(object sender, System.Windows.RoutedEventArgs e) {
            Process.Start("https://www.loserhub.cn/posts/details/92");
        }
    }
}
