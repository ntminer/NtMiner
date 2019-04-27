using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class DriveSet : ViewModelBase {
        private readonly List<Drive> _drives = new List<Drive>();

        public ICommand Apply { get; private set; }

        public DriveSet() {
            foreach (var item in DriveInfo.GetDrives().Where(a => a.DriveType == DriveType.Fixed)) {
                _drives.Add(new Drive(item));
            }
            this.Apply = new DelegateCommand(() => {
                AppContext.Current.VirtualMemorySet.SetVirtualMemoryOfDrive();
            });
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public List<Drive> Drives {
            get {
                return _drives;
            }
        }

        public VirtualMemorySet VirtualMemorySet {
            get {
                return AppContext.Current.VirtualMemorySet;
            }
        }
    }
}
