using NTMiner.Core;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LocalMessagesConfigViewModel : ViewModelBase {

        public LocalMessagesConfigViewModel() {

        }

        public List<KernelOutputKeywordViewModel> KernelOutputKeywordVms {
            get {
                if (NTMinerRoot.Instance.TryGetProfileKernel(out IKernel kernel)) {
                    return AppContext.KernelOutputKeywordViewModels.Instance.GetListByKernelId(kernel.KernelOutputId).ToList();
                }
                return new List<KernelOutputKeywordViewModel>();
            }
        }
    }
}
