using NTMiner.Core.Gpus;
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

        public AppContext.DriveSetViewModel DriveSet {
            get {
                return AppContext.Instance.DriveSetVm;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return AppContext.Instance.GpuVms.Items.Where(a => a.Index != NTMinerRoot.GpuAllId).ToList();
            }
        }

        public List<GpuSetProperty> GpuSetProperties {
            get {
                return NTMinerRoot.Instance.GpuSet.Properties;
            }
        }
    }
}
