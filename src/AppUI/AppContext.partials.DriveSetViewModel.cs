using NTMiner.Vms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class DriveSetViewModel : ViewModelBase {
            public static readonly DriveSetViewModel Instance = new DriveSetViewModel();

            private readonly List<Drive> _drives = new List<Drive>();

            public ICommand Apply { get; private set; }

            private DriveSetViewModel() {
                foreach (var item in DriveInfo.GetDrives().Where(a => a.DriveType == DriveType.Fixed)) {
                    _drives.Add(new Drive(item));
                }
                this.Apply = new DelegateCommand(() => {
                    Current.VirtualMemorySetVm.SetVirtualMemoryOfDrive();
                });
            }

            public AppContext AppContext {
                get {
                    return Current;
                }
            }

            public List<Drive> Drives {
                get {
                    return _drives;
                }
            }

            public VirtualMemorySetViewModel VirtualMemorySet {
                get {
                    return Current.VirtualMemorySetVm;
                }
            }
        }
    }
}
