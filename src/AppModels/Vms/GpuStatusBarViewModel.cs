using System.Linq;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuStatusBarViewModel : ViewModelBase {
        public static readonly GpuStatusBarViewModel Instance = new GpuStatusBarViewModel();

        private GpuStatusBarViewModel() {
#if DEBUG
                NTStopwatch.Start();
#endif
            this.GpuAllVm = AppContext.Instance.GpuVms.Items.FirstOrDefault(a => a.Index == NTMinerRoot.GpuAllId);
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        public GpuViewModel GpuAllVm {
            get; set;
        }

        private Geometry _icon;
        public Geometry Icon {
            get {
                if (_icon == null) {
                    string iconName;
                    switch (NTMinerRoot.Instance.GpuSet.GpuType) {
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
                    _icon = AppUtil.GetResource<Geometry>(iconName);
                }
                return _icon;
            }
        }

        public string IconFill {
            get {
                string iconFill;
                switch (NTMinerRoot.Instance.GpuSet.GpuType) {
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
                return NTMinerRoot.Instance.GpuSet.GpuType.GetDescription();
            }
        }

        public string GpuSetInfo {
            get {
                return NTMinerRoot.Instance.GpuSetInfo;
            }
        }

        public string GpuCountText {
            get {
                return $"x{NTMinerRoot.Instance.GpuSet.Count.ToString()}";
            }
        }
    }
}
