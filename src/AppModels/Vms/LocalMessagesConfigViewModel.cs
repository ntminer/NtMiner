using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LocalMessagesConfigViewModel : ViewModelBase {

        public LocalMessagesConfigViewModel() {

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
