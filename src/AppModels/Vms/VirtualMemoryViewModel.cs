using System;

namespace NTMiner.Vms {
    public class VirtualMemoryViewModel : ViewModelBase {
        public static readonly VirtualMemoryViewModel Empty = new VirtualMemoryViewModel(string.Empty, 0);
        private int _maxSizeMb;

        public VirtualMemoryViewModel(string driveName, int maxSizeMb) {
            this.DriveName = driveName;
            _maxSizeMb = maxSizeMb;
        }

        public string DriveName { get; private set; }
        public int MaxSizeMb {
            get => _maxSizeMb;
            set {
                if (_maxSizeMb != value) {
                    _maxSizeMb = value;
                    OnPropertyChanged(nameof(MaxSizeMb));
                    OnPropertyChanged(nameof(MaxSizeB));
                    OnPropertyChanged(nameof(MaxSizeGb));
                    OnPropertyChanged(nameof(MaxSizeGbText));
                    OnPropertyChanged(nameof(MaxSizeLog2));
                }
            }
        }

        public long MaxSizeB {
            get {
                return MaxSizeMb * ((long)1024 * 1024);
            }
        }

        public double MaxSizeGb {
            get {
                return MaxSizeMb / 1024.0;
            }
        }

        public string MaxSizeGbText {
            get {
                return MaxSizeGb.ToString("f1") + " G"; ;
            }
        }

        public double MaxSizeLog2 {
            get {
                if (MaxSizeMb == 0) {
                    return 0;
                }
                return Math.Log(MaxSizeMb / 1024.0, 2);
            }
            set {
                if (value == 0) {
                    this.MaxSizeMb = 0;
                }
                else {
                    this.MaxSizeMb = (int)(Math.Pow(2, value) * 1024);
                }
                OnPropertyChanged(nameof(MaxSizeLog2));
            }
        }

        public override string ToString() {
            return $"{DriveName}pagefile.sys  {MaxSizeMb.ToString()} {MaxSizeMb.ToString()}";
        }
    }
}
