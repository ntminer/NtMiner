namespace NTMiner.Vms {
    public class LocalMessagesConfigViewModel : ViewModelBase {

        public LocalMessagesConfigViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.BuildEventPath<CurrentMineContextChangedEvent>("挖矿上下文变更后刷新内核输出关键字Vm视图集", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(nameof(KernelOutputVm));
                });
        }

        public KernelOutputViewModel KernelOutputVm {
            get {
                if (NTMinerRoot.Instance.CurrentMineContext == null) {
                    return null;
                }
                if (AppContext.KernelOutputViewModels.Instance.TryGetKernelOutputVm(NTMinerRoot.Instance.CurrentMineContext.KernelOutput.GetId(), out KernelOutputViewModel kernelOutputVm)) {
                    return kernelOutputVm;
                }
                return null;
            }
        }
    }
}
