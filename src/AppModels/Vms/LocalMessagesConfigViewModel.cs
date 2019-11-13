using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LocalMessagesConfigViewModel : ViewModelBase {

        public LocalMessagesConfigViewModel() {

        }

        public List<KernelOutputKeywordViewModel> KernelOutputKeywordVms {
            get {
                if (NTMinerRoot.Instance.CurrentMineContext != null) {
                    return AppContext.KernelOutputKeywordViewModels.Instance.GetListByKernelId(NTMinerRoot.Instance.CurrentMineContext.Kernel.KernelOutputId).ToList();
                }
                return new List<KernelOutputKeywordViewModel>();
            }
        }
    }
}
