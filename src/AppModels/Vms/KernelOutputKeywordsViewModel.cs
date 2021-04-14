using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputKeywordsViewModel : ViewModelBase {

        public ICommand Add { get; private set; }

        public KernelOutputKeywordsViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.BuildEventPath<CurrentMineContextChangedEvent>("挖矿上下文变更后刷新内核输出关键字Vm视图集", LogEnum.DevConsole,
                path: message => {
                    OnPropertyChanged(nameof(KernelOutputVm));
                }, location: this.GetType());
            this.Add = new DelegateCommand(() => {
                KernelOutputViewModel kernelOutputVm = KernelOutputVm;
                if (kernelOutputVm == null) {
                    return;
                }
                // 新建的内核输出关键字时的工作流：默认为Profile级，测试没问题后，然后在界面上提供个按钮转化为Global级提交到服务器
                new KernelOutputKeywordViewModel(Guid.NewGuid(), kernelOutputVm.Id, nameof(LocalMessageType.Info), DataLevel.Profile).Edit.Execute(FormType.Add);
            });
        }

        public KernelOutputViewModel KernelOutputVm {
            get {
                if (NTMinerContext.Instance.CurrentMineContext == null) {
                    return null;
                }
                if (AppRoot.KernelOutputViewModels.Instance.TryGetKernelOutputVm(NTMinerContext.Instance.CurrentMineContext.KernelOutput.GetId(), out KernelOutputViewModel kernelOutputVm)) {
                    return kernelOutputVm;
                }
                return null;
            }
        }
    }
}
