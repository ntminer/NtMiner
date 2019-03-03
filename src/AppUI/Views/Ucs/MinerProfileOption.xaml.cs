using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileOption : UserControl {
        public static string ViewId = nameof(MinerProfileOption);

        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
