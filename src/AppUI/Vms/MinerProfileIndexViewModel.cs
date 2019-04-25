using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileIndexViewModel : ViewModelBase {
        public MinerProfileIndexViewModel() {
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return GpuViewModels.Current.Where(a => a.Index != NTMinerRoot.GpuAllId).OrderBy(a => a.Index).ToList();
            }
        }
    }
}
