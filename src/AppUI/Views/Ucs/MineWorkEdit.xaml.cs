using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MineWorkEdit : UserControl {
        public static void ShowEditWindow(MineWorkViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MineWork"
            }, ucFactory: (window) =>
            {
                MineWorkViewModel vm = new MineWorkViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new MineWorkEdit(vm);
            }, fixedSize: true);
        }

        private MineWorkViewModel Vm {
            get {
                return (MineWorkViewModel)this.DataContext;
            }
        }
        public MineWorkEdit(MineWorkViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
