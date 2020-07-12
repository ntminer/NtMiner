using NTMiner.Gpus;
using NTMiner.Windows;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class OuterPropertyViewModel : ViewModelBase {
        public OuterPropertyViewModel() {

        }

        public string MachineName {
            get {
                return System.Environment.MachineName;
            }
        }

        public OS OS {
            get {
                return OS.Instance;
            }
        }

        public Cpu Cpu {
            get {
                return Cpu.Instance;
            }
        }

        public Ram Ram {
            get {
                return Ram.Instance;
            }
        }

        public AppRoot.DriveSetViewModel DriveSet {
            get {
                return AppRoot.DriveSetVm;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return AppRoot.GpuVms.Items.Where(a => a.Index != NTMinerContext.GpuAllId).ToList();
            }
        }

        public List<GpuSetProperty> GpuSetProperties {
            get {
                return NTMinerContext.Instance.GpuSet.Properties;
            }
        }
    }
}
