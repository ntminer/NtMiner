using NTMiner.Vms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class DriveSetViewModel : ViewModelBase {
            public static readonly DriveSetViewModel Instance = new DriveSetViewModel();

            private readonly List<DriveViewModel> _drives = new List<DriveViewModel>();

            public ICommand Apply { get; private set; }

            private DriveSetViewModel() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                foreach (var item in DriveInfo.GetDrives().Where(a => a.DriveType == DriveType.Fixed)) {
                    _drives.Add(new DriveViewModel(item));
                }
                this.Apply = new DelegateCommand(() => {
                    AppContext.Instance.VirtualMemorySetVm.SetVirtualMemoryOfDrive();
                });
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            public List<DriveViewModel> Drives {
                get {
                    return _drives;
                }
            }

            public VirtualMemorySetViewModel VirtualMemorySet {
                get {
                    return AppContext.Instance.VirtualMemorySetVm;
                }
            }
        }
    }
}
