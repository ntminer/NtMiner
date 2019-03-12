using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientAdd : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_RemoteDesktop"
            }, ucFactory: (window) => {
                MinerClientAddViewModel vm = new MinerClientAddViewModel();
                vm.CloseWindow = () => window.Close();
                return new MinerClientAdd(vm);
            }, fixedSize: true);
        }

        private MinerClientAddViewModel Vm {
            get {
                return (MinerClientAddViewModel)this.DataContext;
            }
        }
        public MinerClientAdd(MinerClientAddViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
