using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LocalMessagesConfigViewModel : ViewModelBase {

        public LocalMessagesConfigViewModel() {
            VirtualRoot.BuildEventPath<CurrentMineContextChangedEvent>("挖矿上下文变更后刷新内核输出关键字Vm视图集", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(nameof(KernelOutputKeywordVms));
                });
        }

        public List<KernelOutputKeywordViewModel> KernelOutputKeywordVms {
            get {
                IMineContext mineContext = NTMinerRoot.Instance.CurrentMineContext;
                if (mineContext != null) {
                    return AppContext.KernelOutputKeywordViewModels.Instance.GetListByKernelId(mineContext.Kernel.KernelOutputId).ToList();
                }
                return new List<KernelOutputKeywordViewModel>();
            }
        }
    }
}
