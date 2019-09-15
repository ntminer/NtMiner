using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileIndexViewModel : ViewModelBase {
        public MinerProfileIndexViewModel() {
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return AppContext.Instance.GpuVms.Where(a => a.Index != NTMinerRoot.GpuAllId).OrderBy(a => a.Index).ToList();
            }
        }
    }
}
