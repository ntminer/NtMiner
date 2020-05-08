using NTMiner.Views;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNameAdd : UserControl {
        public static void ShowWindow(GpuNameViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "添加显卡特征名",
                FormType = FormType.Add,
                IsMaskTheParent = true,
                Width = 320,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Gpu"
            }, ucFactory: (window) => {
                GpuNameViewModel vm = new GpuNameViewModel(source);
                window.AddCloseWindowOnecePath(vm.Id);
                return new GpuNameAdd(vm);
            }, beforeShow: (window, uc) => {
                uc.DoFocus();
            }, fixedSize: true);
        }

        public GpuNameViewModel Vm { get; private set; }

        public GpuNameAdd(GpuNameViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void DoFocus() {
            TbName.Focus();
        }
    }
}
