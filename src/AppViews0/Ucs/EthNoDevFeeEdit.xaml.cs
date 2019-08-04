using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class EthNoDevFeeEdit : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_NoDevFee",
                Title = "配置Claymore ETH反抽水",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new EthNoDevFeeEdit();
                EthNoDevFeeEditViewModel vm = (EthNoDevFeeEditViewModel)uc.DataContext;
                vm.CloseWindow = window.Close;
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
    }
}
