using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputTranslaterEdit : UserControl {
        public static void ShowEditWindow(KernelOutputTranslaterViewModel source) {
            string title;
            if (!DevMode.IsDevMode) {
                title = "翻译器详情";
            }
            else {
                if (NTMinerRoot.Current.KernelOutputTranslaterSet.Contains(source.Id)) {
                    title = "编辑翻译器";
                }
                else {
                    title = "添加翻译器";
                }
            }
            int sortNumber = source.SortNumber;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Coin",
                OnOk = (uc) => {
                    var vm = ((KernelOutputTranslaterEdit)uc).Vm;
                    if (NTMinerRoot.Current.KernelOutputTranslaterSet.Contains(source.Id)) {
                        Global.Execute(new UpdateKernelOutputTranslaterCommand(vm));
                    }
                    else {
                        Global.Execute(new AddKernelOutputTranslaterCommand(vm));
                    }
                    if (sortNumber != vm.SortNumber) {
                        if (KernelViewModels.Current.TryGetKernelVm(vm.KernelId, out KernelViewModel kernelVm)) {
                            kernelVm.OnPropertyChanged(nameof(kernelVm.KernelOutputTranslaters));
                        }
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                KernelOutputTranslaterViewModel vm = new KernelOutputTranslaterViewModel(source.Id).Update(source);
                return new KernelOutputTranslaterEdit(vm);
            }, fixedSize: true);
        }

        private KernelOutputTranslaterViewModel Vm {
            get {
                return (KernelOutputTranslaterViewModel)this.DataContext;
            }
        }
        public KernelOutputTranslaterEdit(KernelOutputTranslaterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
