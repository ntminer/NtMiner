using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class AutoBootStartSeter : UserControl {
        public static void ShowWindow(AutoBootStartSeterViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Option"
            }, ucFactory: (window) => {
                vm.CloseWindow = () => window.Close();
                return new AutoBootStartSeter(vm);
            }, fixedSize: true);
        }

        private AutoBootStartSeterViewModel Vm {
            get {
                return (AutoBootStartSeterViewModel)this.DataContext;
            }
        }
        public AutoBootStartSeter(AutoBootStartSeterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
