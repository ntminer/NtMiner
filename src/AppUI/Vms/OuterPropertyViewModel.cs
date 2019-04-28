using NTMiner.Core.Gpus;
using NTMiner.Windows;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class OuterPropertyViewModel : ViewModelBase {
        public OuterPropertyViewModel() {

        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public string MachineName {
            get {
                return System.Environment.MachineName;
            }
        }

        public OS OS {
            get {
                return OS.Current;
            }
        }

        public Cpu Cpu {
            get {
                return Cpu.Current;
            }
        }

        public Bios Bios {
            get {
                return Bios.Current;
            }
        }

        public Ram Ram {
            get {
                return Ram.Current;
            }
        }

        public AppContext.DriveSetViewModel DriveSet {
            get {
                return AppContext.Current.DriveSet;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return AppContext.Current.GpuVms.Where(a => a.Index != NTMinerRoot.GpuAllId).ToList();
            }
        }

        public List<GpuSetProperty> GpuSetProperties {
            get {
                return NTMinerRoot.Current.GpuSet.Properties;
            }
        }
    }
}
