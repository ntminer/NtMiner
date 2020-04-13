using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class MineWorkEdit : UserControl {
        public static void ShowWindow(FormType formType, MineWorkViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "挖矿作业",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 1000,
                Height = 600,
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

        private MineWorkViewModel Vm {
            get {
                return (MineWorkViewModel)this.DataContext;
            }
        }
        public MineWorkEdit(MineWorkViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
