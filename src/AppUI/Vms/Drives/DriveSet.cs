using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Vms {
    public class DriveSet : ViewModelBase {
        public static readonly DriveSet Current = new DriveSet();

        private readonly List<Drive> _drives = new List<Drive>();

        public DriveSet() {
            foreach (var item in DriveInfo.GetDrives().Where(a => a.DriveType == DriveType.Fixed)) {
                _drives.Add(new Drive(item));
            }
        }

        public void Refresh() {
            OnPropertyChanged(nameof(TotalVirtualMemoryGb));
            OnPropertyChanged(nameof(TotalVirtualMemoryGbText));
        }

        public List<Drive> Drives {
            get {
                return _drives;
            }
        }

        public int TotalVirtualMemoryGb {
            get {
                return _drives.Sum(a => a.VirtualMemory.MaxSizeGb);
            }
        }

        public string TotalVirtualMemoryGbText {
            get {
                return TotalVirtualMemoryGb + " G";
            }
        }
    }
}
