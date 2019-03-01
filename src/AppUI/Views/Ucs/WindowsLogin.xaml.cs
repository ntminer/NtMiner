using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WindowsLogin : UserControl {
        public static void ShowWindow(WindowsLoginViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_RemoteDesktop"
            }, ucFactory: (window) => {
                WindowsLoginViewModel vm = source;
                vm.CloseWindow = () => window.Close();
                return new WindowsLogin(vm);
            }, fixedSize: true);
        }

        private WindowsLoginViewModel Vm {
            get {
                return (WindowsLoginViewModel)this.DataContext;
            }
        }
        public WindowsLogin(WindowsLoginViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
