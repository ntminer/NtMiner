using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerGroupEdit : UserControl {
        public static void ShowWindow(FormType formType, MinerGroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "矿工分组",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 520,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerGroup"
            }, ucFactory: (window) => {
                MinerGroupViewModel vm = new MinerGroupViewModel(source);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new MinerGroupEdit(vm);
            }, beforeShow: (window, uc) => {
                uc.DoFocus();
            }, fixedSize: true);
        }

        public MinerGroupViewModel Vm { get; private set; }

        public MinerGroupEdit(MinerGroupViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void DoFocus() {
            if (string.IsNullOrEmpty(Vm.Name)) {
                TbName.Focus();
            }
            else if (string.IsNullOrEmpty(Vm.Description)) {
                MTbDescription.Focus();
            }
        }
    }
}
