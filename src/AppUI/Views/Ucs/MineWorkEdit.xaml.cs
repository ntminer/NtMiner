using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MineWorkEdit : UserControl {
        public static void ShowWindow(FormType formType, MineWorkViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_MineWork",
                OnClose = (uc) => {
                    MineWorkViewModel vm = (MineWorkViewModel)uc.DataContext;
                    vm.Save.Execute(null);
                }
            }, ucFactory: (window) =>
            {
                MineWorkViewModel vm = new MineWorkViewModel(source);
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
