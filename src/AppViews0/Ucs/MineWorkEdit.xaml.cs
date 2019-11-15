using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MineWorkEdit : UserControl {
        public static void ShowWindow(FormType formType, MineWorkViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "矿工",
                FormType = formType,
                IsDialogWindow = true,
                Width = 900,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                IconName = "Icon_MineWork"
            }, ucFactory: (window) => {
                MineWorkViewModel vm = new MineWorkViewModel(source);
                window.Closed += (sender, e) => {
                    vm.Save.Execute(null);
                };
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
        }
    }
}
