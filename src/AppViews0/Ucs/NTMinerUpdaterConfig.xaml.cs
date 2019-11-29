using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class NTMinerUpdaterConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "升级器版本",
                IconName = "Icon_Update",
                Width = 500,
                Height = 180,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new NTMinerUpdaterConfig();
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: uc.Vm.Id, location: typeof(NTMinerUpdaterConfig));
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
