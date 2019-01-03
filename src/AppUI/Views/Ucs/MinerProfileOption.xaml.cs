using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileOption : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "选项",
                IconName = "Icon_MinerProfile",
                Width = 450,
                Height = 360,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new MinerProfileOption(), fixedSize: true);
        }

        public MinerProfileViewModel Vm {
            get {
                return (MinerProfileViewModel)this.DataContext;
            }
        }

        public MinerProfileOption() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(MinerProfileOption), this.Resources);
        }
    }
}
