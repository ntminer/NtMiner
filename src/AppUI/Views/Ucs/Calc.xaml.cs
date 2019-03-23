using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Calc : UserControl {
        public static string ViewId = nameof(Calc);

        public static void ShowWindow(CoinViewModel coin) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Calc",
                Width = 650,
                Height = 300,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterText = "数据来自鱼池首页，感谢鱼池的支持"
            }, ucFactory: (window) => {
                var uc = new Calc(coin);
                return uc;
            }, fixedSize: true);
        }

        public CalcViewModel Vm {
            get {
                return (CalcViewModel)this.DataContext;
            }
        }

        private Calc(CoinViewModel coin) {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Vm.SelectedCoinVm = coin;
        }
    }
}
