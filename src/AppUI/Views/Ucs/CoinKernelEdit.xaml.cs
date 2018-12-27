using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelEdit : UserControl {
        public static void ShowEditWindow(CoinKernelViewModel source) {
            string title;
            if (!DevMode.IsDevMode) {
                title = "内核详情";
            }
            else {
                if (NTMinerRoot.Current.CoinKernelSet.Contains(source.Id)) {
                    title = "编辑内核";
                }
                else {
                    title = "添加内核";
                }
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
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
        }
    }
}
