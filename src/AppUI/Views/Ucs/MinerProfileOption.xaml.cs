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

        private void ButtonHotKey_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z) {
                Vm.HotKey = e.Key.ToString();
            }
        }
    }
}
