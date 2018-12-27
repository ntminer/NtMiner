using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputFilterEdit : UserControl {
        public static void ShowEditWindow(KernelOutputFilterViewModel source) {
            string title;
            if (!DevMode.IsDevMode) {
                title = "过滤器详情";
            }
            else {
                if (NTMinerRoot.Current.KernelOutputFilterSet.Contains(source.Id)) {
                    title = "编辑过滤器";
                }
                else {
                    title = "添加过滤器";
                }
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
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
