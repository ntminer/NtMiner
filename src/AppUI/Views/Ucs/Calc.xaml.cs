using NTMiner.Core;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Calc : UserControl {
        public static void ShowWindow(CoinViewModel coin) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "收益计算器",
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

        private readonly List<Bus.IDelegateHandler> _handlers = new List<Bus.IDelegateHandler>();
        private Calc(CoinViewModel coin) {
            InitializeComponent();
            Vm.SelectedCoinVm = coin;
            VirtualRoot.On<CalcConfigSetInitedEvent>("收益计算器数据集刷新后刷新VM", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(()=> {
                        Vm.ReRender();
                    });
                }).AddToCollection(_handlers).AddToCollection(AppContext.ContextHandlers);
            this.Unloaded += CalcConfig_Unloaded;
        }

        private void CalcConfig_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var handler in _handlers) {
                VirtualRoot.UnPath(handler);
            }
        }
    }
}
