using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientSetting : UserControl {
        public static void ShowWindow(MinerClientSettingViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Setting"
            }, ucFactory: (window) => {
                vm.CloseWindow = () => window.Close();
                return new MinerClientSetting(vm);
            }, fixedSize: true);
        }

        private MinerClientSettingViewModel Vm {
            get {
                return (MinerClientSettingViewModel)this.DataContext;
            }
        }
        public MinerClientSetting(MinerClientSettingViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
