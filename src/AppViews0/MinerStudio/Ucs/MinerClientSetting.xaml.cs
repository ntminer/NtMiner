using NTMiner.MinerStudio.Vms;
using System.Windows;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class MinerClientSetting : BlankWindow {
        public static void ShowWindow(MinerClientSettingViewModel vm) {
            Window window = new MinerClientSetting(vm);
            window.AddCloseWindowOnecePath(vm.Id);
            window.MousePosition();
            window.ShowSoftDialog();
        }

        public MinerClientSetting(MinerClientSettingViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            var owner = WpfUtil.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
        }
    }
}
