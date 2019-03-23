using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class OverClockDataEdit : UserControl {
        public static void ShowWindow(FormType formType, OverClockDataViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_OverClock"
            }, ucFactory: (window) => {
                OverClockDataViewModel vm = new OverClockDataViewModel(source);
                vm.CloseWindow = () => window.Close();
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
