using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输出",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 900,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_KernelOutput"
            }, ucFactory: (window) =>
            {
                KernelOutputViewModel vm = new KernelOutputViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(KernelOutputEdit));
                return new KernelOutputEdit(vm);
            }, fixedSize: true);
        }

        private KernelOutputViewModel Vm {
            get {
                return (KernelOutputViewModel)this.DataContext;
            }
        }

        public KernelOutputEdit(KernelOutputViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
