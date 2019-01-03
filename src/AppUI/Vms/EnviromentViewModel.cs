using NTMiner.Core.Gpus;
using NTMiner.Windows;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class EnviromentViewModel : ViewModelBase {
        public static readonly EnviromentViewModel Current = new EnviromentViewModel();

        private EnviromentViewModel() {

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

        public DrivesViewModel DrivesVm {
            get {
                return DrivesViewModel.Current;
            }
        }
        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return GpuStatusBarViewModel.Current;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return GpuViewModels.Current.Where(a => a.Index != NTMinerRoot.Current.GpuAllId).ToList();
            }
        }

        public List<GpuSetProperty> GpuSetProperties {
            get {
                return NTMinerRoot.Current.GpuSet.Properties;
            }
        }
    }
}
