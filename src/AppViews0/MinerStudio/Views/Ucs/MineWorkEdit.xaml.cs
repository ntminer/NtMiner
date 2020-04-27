using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MineWorkEdit : UserControl {
        public static void ShowWindow(FormType formType, MineWorkViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "挖矿作业 — 作业通常用于让不同的矿机执行同样的挖矿任务",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 900,
                Height = 560,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_MineWork"
            }, ucFactory: (window) => {
                MineWorkViewModel vm = new MineWorkViewModel(source);
                window.Closed += (sender, e) => {
                    vm.Save.Execute(null);
                };
                NotiCenterWindow.Bind(window);
                return new MineWorkEdit(vm);
            }, beforeShow: (window, uc)=> {
                NTMinerContext.RefreshArgsAssembly.Invoke("打开编辑挖矿作业页面时");
            }, fixedSize: false);
        }

        public MineWorkViewModel Vm { get; private set; }
        public MineWorkEdit(MineWorkViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
