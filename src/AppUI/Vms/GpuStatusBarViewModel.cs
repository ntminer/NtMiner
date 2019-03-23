using System.Linq;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuStatusBarViewModel : ViewModelBase {
        public static readonly GpuStatusBarViewModel Current = new GpuStatusBarViewModel();

        public GpuStatusBarViewModel() {
            this.GpuAllVm = GpuViewModels.Current.FirstOrDefault(a => a.Index == NTMinerRoot.GpuAllId);
        }

        public GpuViewModel GpuAllVm {
            get; set;
        }

        private Geometry _icon;
        public Geometry Icon {
            get {
                if (_icon == null) {
                    string iconName;
                    switch (NTMinerRoot.Current.GpuSet.GpuType) {
                        case GpuType.NVIDIA:
                            iconName = "Icon_Nvidia";
                            break;
                        case GpuType.AMD:
                            iconName = "Icon_AMD";
                            break;
                        default:
                            iconName = "Icon_GpuEmpty";
                            break;
                    }
                    _icon = (Geometry)System.Windows.Application.Current.Resources[iconName];
                }
                return _icon;
            }
        }

        public string IconFill {
            get {
                string iconFill;
                switch (NTMinerRoot.Current.GpuSet.GpuType) {
                    case GpuType.NVIDIA:
                        iconFill = "Green";
                        break;
                    case GpuType.AMD:
                        iconFill = "Red";
                        break;
                    case GpuType.Empty:
                    default:
                        iconFill = "Gray";
                        break;
                }
                return iconFill;
            }
        }

        public string GpuSetName {
            get {
                return NTMinerRoot.Current.GpuSet.GpuType.GetDescription();
            }
        }

        public string GpuSetInfo {
            get {
                return NTMinerRoot.Current.GpuSetInfo;
            }
        }

        public string GpuCountText {
            get {
                return $"({NTMinerRoot.Current.GpuSet.Count} GPU)";
            }
        }
    }
}
