using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class RemoteLogin : UserControl {
        public static void ShowWindow(RemoteLoginViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_RemoteDesktop"
            }, ucFactory: (window) => {
                RemoteLoginViewModel vm = source;
                vm.CloseWindow = () => window.Close();
                return new RemoteLogin(vm);
            }, fixedSize: true);
        }

        private RemoteLoginViewModel Vm {
            get {
                return (RemoteLoginViewModel)this.DataContext;
            }
        }
        public RemoteLogin(RemoteLoginViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
