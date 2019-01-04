using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputFilterEdit : UserControl {
        public static void ShowEditWindow(KernelOutputFilterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin",
                OnOk = (uc) => {
                    var vm = ((KernelOutputFilterEdit)uc).Vm;
                    if (NTMinerRoot.Current.KernelOutputFilterSet.Contains(source.Id)) {
                        Global.Execute(new UpdateKernelOutputFilterCommand(vm));
                    }
                    else {
                        Global.Execute(new AddKernelOutputFilterCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                KernelOutputFilterViewModel vm = new KernelOutputFilterViewModel(source.Id).Update(source);
                return new KernelOutputFilterEdit(vm);
            }, fixedSize: true);
        }

        private KernelOutputFilterViewModel Vm {
            get {
                return (KernelOutputFilterViewModel)this.DataContext;
            }
        }
        public KernelOutputFilterEdit(KernelOutputFilterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
