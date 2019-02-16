using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class VirtualMemory : ViewModelBase {
        public static readonly VirtualMemory Empty = new VirtualMemory {
            DriveName = string.Empty,
            MaxSizeMb = 0
        };
        private static readonly Dictionary<string, VirtualMemory> _initialVms = new Dictionary<string, VirtualMemory>();
        private int _maxSizeMb;

        static VirtualMemory() {
            foreach (var item in GetPagingFiles()) {
                _initialVms.Add(item.DriveName, item);
            }
            foreach (var drive in DriveSet.Current.Drives) {
                VirtualMemories.Instance.Add(drive.Name, new VirtualMemory {
                    DriveName = drive.Name,
                    _maxSizeMb = 0
                });
            }
            foreach (var item in GetPagingFiles()) {
                VirtualMemory virtualMemory = VirtualMemories.Instance[item.DriveName];
                virtualMemory.MaxSizeMb = item.MaxSizeMb;
            }
        }

        public static bool IsStateChanged {
            get {
                foreach (var item in _initialVms) {
                    if (!VirtualMemories.Instance.Contains(item.Key)) {
                        return true;
                    }
                }
                foreach (var item in VirtualMemories.Instance) {
                    if (!_initialVms.ContainsKey(item.DriveName)) {
                        return true;
                    }
                }
                foreach (var item in _initialVms) {
                    if (VirtualMemories.Instance[item.Key].MaxSizeB != item.Value.MaxSizeB) {
                        return true;
                    }
                }
                return false;
            }
        }

        private VirtualMemory() {

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

        public int MaxSizeGb {
            get {
                return MaxSizeMb / 1024;
            }
        }

        public string MaxSizeGbText {
            get {
                return MaxSizeGb + " G"; ;
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
            return $"{DriveName}pagefile.sys  {MaxSizeMb} {MaxSizeMb}";
        }

        private const string MemoryManagementSubKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
        public static void SetVirtualMemoryOfDrive() {
            List<VirtualMemory> virtualMemories = VirtualMemories.Instance.Where(a => a.MaxSizeMb != 0).ToList();
            string[] value = virtualMemories.Select(a => a.ToString()).ToArray();

            Windows.Registry.SetValue(Microsoft.Win32.Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles", value);
            DriveSet.Current.Refresh();
            DrivesViewModel.Current.IsNeedRestartWindows = IsStateChanged;
        }

        private static List<VirtualMemory> GetPagingFiles() {
            object value = Windows.Registry.GetValue(Microsoft.Win32.Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles");
            // REG_SZ or REG_MULTI_SZ
            List<VirtualMemory> list;
            if (value is string[]) {
                list = Parse((string[])value);
            }
            else {
                list = new List<VirtualMemory>();
            }
            return list;
        }

        private static VirtualMemory Parse(string vmReg) {
            string driveName;
            int minsize = 0;
            int maxsize = 0;
            try {
                string[] strarr = vmReg.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (strarr.Length == 3) {
                    driveName = strarr[0].Substring(0, 3);
                    minsize = Convert.ToInt32(strarr[1]);
                    maxsize = Convert.ToInt32(strarr[2]);
                    return new VirtualMemory {
                        DriveName = driveName,
                        MaxSizeMb = maxsize
                    };
                }
                return null;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        private static List<VirtualMemory> Parse(string[] vmReg) {
            List<VirtualMemory> list = new List<VirtualMemory>();
            try {
                foreach (string item in vmReg) {
                    VirtualMemory vm = Parse(item);
                    if (vm != null) {
                        list.Add(vm);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return list;
        }
    }
}
