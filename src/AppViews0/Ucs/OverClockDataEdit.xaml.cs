using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class OverClockDataEdit : UserControl {
        public static void ShowWindow(FormType formType, OverClockDataViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "超频菜谱",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 250,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_OverClock"
            }, ucFactory: (window) => {
                OverClockDataViewModel vm = new OverClockDataViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(OverClockDataEdit));
                return new OverClockDataEdit(vm);
            }, fixedSize: true);
        }

        private OverClockDataViewModel Vm {
            get {
                return (OverClockDataViewModel)this.DataContext;
            }
        }
        public OverClockDataEdit(OverClockDataViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
