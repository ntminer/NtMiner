using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SysDicItemEdit : UserControl {
        public static void ShowWindow(FormType formType, SysDicItemViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "系统字典项",
                FormType = formType,
                Width= 520,
                IsMaskTheParent = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_SysDic"
            }, ucFactory: (window) =>
            {
                SysDicItemViewModel vm = new SysDicItemViewModel(source);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new SysDicItemEdit(vm);
            }, fixedSize: true);
        }

        public SysDicItemViewModel Vm { get; private set; }

        public SysDicItemEdit(SysDicItemViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
