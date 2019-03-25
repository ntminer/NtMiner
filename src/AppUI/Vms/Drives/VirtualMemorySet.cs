using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class VirtualMemorySet : ViewModelBase, IEnumerable<VirtualMemory> {
        public static readonly VirtualMemorySet Instance = new VirtualMemorySet();

        private readonly Dictionary<string, VirtualMemory> _dic = new Dictionary<string, VirtualMemory>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, VirtualMemory> _initialVms = new Dictionary<string, VirtualMemory>();
        private VirtualMemorySet() {
            foreach (var item in GetPagingFiles()) {
                _initialVms.Add(item.DriveName, item);
            }
            foreach (var drive in DriveSet.Current.Drives) {
                _dic.Add(drive.Name, new VirtualMemory(drive.Name, 0));
            }
            foreach (var item in GetPagingFiles()) {
                if (_dic.TryGetValue(item.DriveName, out VirtualMemory vm)) {
                    vm.MaxSizeMb = item.MaxSizeMb;
                }
            }
            NTMinerRoot.OSVirtualMemoryMb = _dic.Values.Sum(a => a.MaxSizeMb);
        }

        public bool IsStateChanged {
            get {
                foreach (var item in _dic.Values.Where(a => a.MaxSizeMb != 0)) {
                    if (!_initialVms.ContainsKey(item.DriveName)) {
                        return true;
                    }
                }
                foreach (var item in _initialVms) {
                    if (_dic[item.Key].MaxSizeB != item.Value.MaxSizeB) {
                        return true;
                    }
                }
                return false;
            }
        }


        public int TotalVirtualMemoryGb {
            get {
                return _dic.Values.Sum(a => a.MaxSizeGb);
            }
        }

        public string TotalVirtualMemoryGbText {
            get {
                return TotalVirtualMemoryGb + " G";
            }
        }

        private const string MemoryManagementSubKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
        public void SetVirtualMemoryOfDrive() {
            List<VirtualMemory> virtualMemories = _dic.Values.Where(a => a.MaxSizeMb != 0).ToList();
            string[] value = virtualMemories.Select(a => a.ToString()).ToArray();

            Windows.Registry.SetValue(Microsoft.Win32.Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles", value);
            OnPropertyChanged(nameof(TotalVirtualMemoryGb));
            OnPropertyChanged(nameof(TotalVirtualMemoryGbText));
            OnPropertyChanged(nameof(IsStateChanged));
            NTMinerRoot.OSVirtualMemoryMb = _dic.Values.Sum(a => a.MaxSizeMb);
        }

        private List<VirtualMemory> GetPagingFiles() {
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

        private VirtualMemory Parse(string vmReg) {
            string driveName;
            int minsize = 0;
            int maxsize = 0;
            try {
                string[] strarr = vmReg.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (strarr.Length == 3) {
                    driveName = strarr[0].Substring(0, 3);
                    minsize = Convert.ToInt32(strarr[1]);
                    maxsize = Convert.ToInt32(strarr[2]);
                    return new VirtualMemory(driveName, maxsize);
                }
                return null;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        private List<VirtualMemory> Parse(string[] vmReg) {
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

        public VirtualMemory this[string driveName] {
            get {
                return _dic[driveName];
            }
        }

        public bool Contains(string driveName) {
            return _dic.ContainsKey(driveName);
        }

        public IEnumerator<VirtualMemory> GetEnumerator() {
            return _dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dic.Values.GetEnumerator();
        }
    }
}
