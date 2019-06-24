using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerNamesSeter : UserControl {
        public static void ShowWindow(MinerNamesSeterViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "群控矿工名",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerName"
            }, ucFactory: (window) => {
                vm.CloseWindow = () => window.Close();
                return new MinerNamesSeter(vm);
            }, fixedSize: true);
        }

        private MinerNamesSeterViewModel Vm {
            get {
                return (MinerNamesSeterViewModel)this.DataContext;
            }
        }
        public MinerNamesSeter(MinerNamesSeterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
