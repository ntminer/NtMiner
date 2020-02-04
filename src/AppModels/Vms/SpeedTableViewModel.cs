using NTMiner.Core;
using System.Windows;

namespace NTMiner.Vms {
    public class SpeedTableViewModel : ViewModelBase {
        public SpeedTableViewModel() {
        }

        public Visibility IsVoltVisible {
            get {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public AppContext.GpuSpeedViewModels GpuSpeedVms {
            get {
                return AppContext.GpuSpeedViewModels.Instance;
            }
        }
    }
}
