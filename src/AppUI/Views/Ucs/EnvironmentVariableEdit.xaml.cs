using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class EnvironmentVariableEdit : UserControl {
        public static void ShowWindow(CoinKernelViewModel coinKernelVm, EnvironmentVariable environmentVariable) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "环境变量",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                EnvironmentVariableEditViewModel vm = new EnvironmentVariableEditViewModel(coinKernelVm, environmentVariable) {
                    CloseWindow = () => window.Close()
                };
                return new EnvironmentVariableEdit(vm);
            }, fixedSize: true);
        }

        private EnvironmentVariableEditViewModel Vm {
            get {
                return (EnvironmentVariableEditViewModel)this.DataContext;
            }
        }

        public EnvironmentVariableEdit(EnvironmentVariableEditViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
