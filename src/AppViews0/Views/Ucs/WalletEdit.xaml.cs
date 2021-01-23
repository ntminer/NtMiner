using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WalletEdit : UserControl {
        public static void ShowWindow(FormType formType, WalletViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "钱包",
                FormType = formType,
                Width = 520,
                IconName = "Icon_Wallet",
                IsMaskTheParent = true,
                FooterVisible = System.Windows.Visibility.Collapsed,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                WalletViewModel vm = new WalletViewModel(source) {
                    AfterClose = source.AfterClose
                };
                window.BuildCloseWindowOnecePath(vm.Id);
                return new WalletEdit(vm);
            }, fixedSize: true);
        }

        public WalletViewModel Vm { get; private set; }

        public WalletEdit(WalletViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
