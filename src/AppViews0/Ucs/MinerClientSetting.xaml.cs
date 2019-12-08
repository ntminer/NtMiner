using NTMiner.Vms;
using System.Windows;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientSetting : BlankWindow {
        public static void ShowWindow(MinerClientSettingViewModel vm) {
            Window window = new MinerClientSetting(vm);
            window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                window.Close();
            }, pathId: vm.Id, location: typeof(MinerClientSetting));
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
