using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientFinderConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "矿机雷达版本",
                IconName = "Icon_MinerClientFinder",
                Width = 500,
                Height = 180,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new MinerClientFinderConfig();
                window.AddCloseWindowOnecePath(uc.Vm.Id);
                return uc;
            }, fixedSize: true);
        }

        public MinerClientFinderConfigViewModel Vm {
            get {
                return (MinerClientFinderConfigViewModel)this.DataContext;
            }
        }

        public MinerClientFinderConfig() {
            InitializeComponent();
        }
    }
}
