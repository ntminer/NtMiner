using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerNamesSeter : UserControl {
        public static void ShowWindow(MinerNamesSeterViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "群控矿工名",
                IsMaskTheParent = true,
                Width = 270,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerName"
            }, ucFactory: (window) => {
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(MinerNamesSeter));
                return new MinerNamesSeter(vm);
            }, fixedSize: true);
        }

        public MinerNamesSeter(MinerNamesSeterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
