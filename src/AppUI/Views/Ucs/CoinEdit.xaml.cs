using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinEdit : UserControl {
        public static void ShowEditWindow(CoinViewModel source) {
            string title;
            if (!DevMode.IsDevMode) {
                title = "币种详情";
            }
            else {
                if (NTMinerRoot.Current.CoinSet.Contains(source.Id)) {
                    title = "编辑币种";
                }
                else {
                    title = "添加币种";
                }
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
                IsDialogWindow = true,
                SaveVisible = DevMode.IsDevMode? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin",
                OnOk = (uc) => {
                    CoinViewModel vm = ((CoinEdit)uc).Vm;
                    if (NTMinerRoot.Current.CoinSet.Contains(source.Id)) {
                        Global.Execute(new UpdateCoinCommand(vm));
                    }
                    else {
                        Global.Execute(new AddCoinCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                CoinViewModel vm = new CoinViewModel(source);
                return new CoinEdit(vm);
            }, fixedSize: true);
        }

        private CoinViewModel Vm {
            get {
                return (CoinViewModel)this.DataContext;
            }
        }
        public CoinEdit(CoinViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
