using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelSelect : UserControl {
        public static void ShowWindow(CoinViewModel coin) {
            KernelSelect uc = null;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                uc = new KernelSelect();
                uc.Vm.ExceptedCoin = coin;
                return uc;
            }, fixedSize: true);
            if (uc != null) {
                if (uc.Vm.SelectedResult != null) {
                    int sortNumber = coin.CoinKernels.Count == 0 ? 1 : coin.CoinKernels.Max(a => a.SortNumber) + 1;
                    VirtualRoot.Execute(new AddCoinKernelCommand(new CoinKernelViewModel(Guid.NewGuid()) {
                        Args = string.Empty,
                        CoinId = coin.Id,
                        Description = string.Empty,
                        KernelId = uc.Vm.SelectedResult.Id,
                        SortNumber = sortNumber
                    }));
                }
            }
        }

        public KernelSelectViewModel Vm {
            get {
                return (KernelSelectViewModel)this.DataContext;
            }
        }

        public KernelSelect() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
