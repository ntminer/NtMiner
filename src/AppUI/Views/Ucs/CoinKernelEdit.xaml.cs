using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelEdit : UserControl {
        public static void ShowEditWindow(CoinKernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                IconName = "Icon_Kernel",
                SaveVisible = DevMode.IsDevMode ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                CloseVisible = System.Windows.Visibility.Visible,
                OnOk = (uc) => {
                    var vm = ((CoinKernelEdit)uc).Vm;
                    if (NTMinerRoot.Current.CoinKernelSet.Contains(source.Id)) {
                        Global.Execute(new UpdateCoinKernelCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                CoinKernelViewModel vm = new CoinKernelViewModel(source);
                return new CoinKernelEdit(vm);
            }, fixedSize: true);
        }

        private CoinKernelViewModel Vm {
            get {
                return (CoinKernelViewModel)this.DataContext;
            }
        }

        public CoinKernelEdit(CoinKernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(CoinKernelEdit), this.Resources);
        }
    }
}
