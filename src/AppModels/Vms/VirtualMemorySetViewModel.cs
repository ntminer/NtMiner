using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class VirtualMemorySetViewModel : ViewModelBase, IEnumerable<VirtualMemoryViewModel> {
        public static readonly VirtualMemorySetViewModel Instance = new VirtualMemorySetViewModel();

        private readonly Dictionary<string, VirtualMemoryViewModel> _dic = new Dictionary<string, VirtualMemoryViewModel>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, VirtualMemoryViewModel> _initialVms = new Dictionary<string, VirtualMemoryViewModel>(StringComparer.OrdinalIgnoreCase);
        private VirtualMemorySetViewModel() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
            foreach (var item in GetPagingFiles()) {
                _initialVms.Add(item.DriveName, item);
            }
            foreach (var drive in AppContext.Instance.DriveSetVm.Drives) {
                _dic.Add(drive.Name, new VirtualMemoryViewModel(drive.Name, 0));
            }
            foreach (var item in _initialVms.Values) {
                if (_dic.TryGetValue(item.DriveName, out VirtualMemoryViewModel vm)) {
                    vm.MaxSizeMb = item.MaxSizeMb;
                }
            }
            NTMinerRoot.OSVirtualMemoryMb = _dic.Values.Sum(a => a.MaxSizeMb);
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
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
            List<VirtualMemoryViewModel> virtualMemories = _dic.Values.Where(a => a.MaxSizeMb != 0).ToList();
            string[] value = virtualMemories.Select(a => a.ToString()).ToArray();

            Windows.WinRegistry.SetValue(Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles", value);
            OnPropertyChanged(nameof(TotalVirtualMemoryGb));
            OnPropertyChanged(nameof(TotalVirtualMemoryGbText));
            OnPropertyChanged(nameof(IsStateChanged));
            NTMinerRoot.OSVirtualMemoryMb = _dic.Values.Sum(a => a.MaxSizeMb);
        }

        private List<VirtualMemoryViewModel> GetPagingFiles() {
            object value = Windows.WinRegistry.GetValue(Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles");
            // REG_SZ or REG_MULTI_SZ
            List<VirtualMemoryViewModel> list;
            if (value is string[]) {
                list = Parse((string[])value);
            }
            else {
                list = new List<VirtualMemoryViewModel>();
            }
            return list;
        }

        private VirtualMemoryViewModel Parse(string vmReg) {
            string driveName;
            int minsize = 0;
            int maxsize = 0;
            try {
                string[] strarr = vmReg.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (strarr.Length == 3) {
                    driveName = strarr[0].Substring(0, 3);
                    minsize = Convert.ToInt32(strarr[1]);
                    maxsize = Convert.ToInt32(strarr[2]);
                    return new VirtualMemoryViewModel(driveName, maxsize);
                }
                return null;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        private List<VirtualMemoryViewModel> Parse(string[] vmReg) {
            List<VirtualMemoryViewModel> list = new List<VirtualMemoryViewModel>();
            try {
                foreach (string item in vmReg) {
                    VirtualMemoryViewModel vm = Parse(item);
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

        public VirtualMemoryViewModel this[string driveName] {
            get {
                return _dic[driveName];
            }
        }

        public bool Contains(string driveName) {
            return _dic.ContainsKey(driveName);
        }

        public IEnumerator<VirtualMemoryViewModel> GetEnumerator() {
            return _dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dic.Values.GetEnumerator();
        }
    }
}
