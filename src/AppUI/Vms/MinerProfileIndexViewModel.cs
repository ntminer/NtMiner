using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileIndexViewModel : ViewModelBase {
        public MinerProfileIndexViewModel() {
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Current.MinerProfileVm;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return AppContext.GpuVms.Where(a => a.Index != NTMinerRoot.GpuAllId).OrderBy(a => a.Index).ToList();
            }
        }
    }
}
