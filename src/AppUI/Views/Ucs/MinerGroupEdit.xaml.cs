using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerGroupEdit : UserControl {
        public static void ShowWindow(FormType formType, MinerGroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerGroup"
            }, ucFactory: (window) => {
                MinerGroupViewModel vm = new MinerGroupViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new MinerGroupEdit(vm);
            }, fixedSize: true);
        }

        private MinerGroupViewModel Vm {
            get {
                return (MinerGroupViewModel)this.DataContext;
            }
        }
        public MinerGroupEdit(MinerGroupViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
