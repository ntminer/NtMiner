using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class SpeedChartViewModels : IEnumerable<SpeedChartViewModel> {
        private readonly Dictionary<int, SpeedChartViewModel> _dicByGpuIndex = new Dictionary<int, SpeedChartViewModel>();

        public SpeedChartViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            if (AppContext.Instance.MinerProfileVm.CoinVm != null) {
                foreach (var item in AppContext.Instance.GpuSpeedVms.All) {
                    _dicByGpuIndex.Add(item.GpuVm.Index, new SpeedChartViewModel(item));
                }
            }
        }

        public bool ContainsKey(int gpuIndex) {
            return _dicByGpuIndex.ContainsKey(gpuIndex);
        }

        public SpeedChartViewModel this[int index] {
            get {
                if (_dicByGpuIndex.ContainsKey(index)) {
                    return _dicByGpuIndex[index];
                }
                return null;
            }
        }

        public IEnumerator<SpeedChartViewModel> GetEnumerator() {
            return _dicByGpuIndex.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dicByGpuIndex.Values.GetEnumerator();
        }
    }
}
