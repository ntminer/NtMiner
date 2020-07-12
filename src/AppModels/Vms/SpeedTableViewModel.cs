using NTMiner.Gpus;
using System.Windows;

namespace NTMiner.Vms {
    public class SpeedTableViewModel : ViewModelBase {
        public SpeedTableViewModel() {
        }

        public Visibility IsVoltVisible {
            get {
                if (NTMinerContext.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public AppRoot.GpuSpeedViewModels GpuSpeedVms {
            get {
                return AppRoot.GpuSpeedViewModels.Instance;
            }
        }
    }
}
