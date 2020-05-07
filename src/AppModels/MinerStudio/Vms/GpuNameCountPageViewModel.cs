using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class GpuNameCountPageViewModel : ViewModelBase {
        private List<GpuNameCountViewModel> _gpuNameCounts;

        public GpuNameCountPageViewModel() {
            this.Refresh();
        }

        public void Refresh() {
            RpcRoot.OfficialServer.GpuNameService.GetGpuNameCountsAsync((response, e) => {
                this.GpuNameCounts = response.Data.Select(a => new GpuNameCountViewModel(a)).ToList();
            });
        }

        public List<GpuNameCountViewModel> GpuNameCounts {
            get => _gpuNameCounts;
            set {
                if (_gpuNameCounts != value) {
                    _gpuNameCounts = value;
                    OnPropertyChanged(nameof(GpuNameCounts));
                }
            }
        }
    }
}
