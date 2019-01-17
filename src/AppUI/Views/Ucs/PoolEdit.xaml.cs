using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolEdit : UserControl {
        public static void ShowEditWindow(PoolViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Pool",
                IsDialogWindow = true,
                Width = 540,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                PoolViewModel vm = new PoolViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new PoolEdit(vm);
            }, fixedSize: true);
        }

        private PoolViewModel Vm {
            get {
                return (PoolViewModel)this.DataContext;
            }
        }

        public PoolEdit(PoolViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
