using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class NTMinerUpdaterConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "升级器版本",
                IconName = "Icon_Update",
                Width = 500,
                Height = 180,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new NTMinerUpdaterConfig();
                window.AddCloseWindowOnecePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public NTMinerUpdaterConfigViewModel Vm {
            get {
                return (NTMinerUpdaterConfigViewModel)this.DataContext;
            }
        }

        public NTMinerUpdaterConfig() {
            InitializeComponent();
        }
    }
}
