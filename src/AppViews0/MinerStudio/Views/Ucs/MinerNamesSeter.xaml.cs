using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerNamesSeter : UserControl {
        public static void ShowWindow(MinerNamesSeterViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "群控名",
                IsMaskTheParent = true,
                Width = 270,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerName"
            }, ucFactory: (window) => {
                window.AddCloseWindowOnecePath(vm.Id);
                return new MinerNamesSeter(vm);
            }, fixedSize: true);
        }

        public MinerNamesSeter(MinerNamesSeterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
