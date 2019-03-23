using NTMiner.Core.Gpus;
using NTMiner.Windows;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class OuterPropertyViewModel : ViewModelBase {
        public static readonly OuterPropertyViewModel Current = new OuterPropertyViewModel();

        private OuterPropertyViewModel() {

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

        public Processor Processor {
            get {
                return Processor.Current;
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

        public DriveSet DriveSet {
            get {
                return DriveSet.Current;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return GpuViewModels.Current.Where(a => a.Index != NTMinerRoot.GpuAllId).ToList();
            }
        }

        public List<GpuSetProperty> GpuSetProperties {
            get {
                return NTMinerRoot.Current.GpuSet.Properties;
            }
        }
    }
}
