using Microsoft.Win32;
using NTMiner.VirtualMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTMiner {
    public static partial class VirtualRoot {
        public interface IDriveSet {
            int OSVirtualMemoryMb { get; }
            string ToDiskSpaceString();
            IEnumerable<DriveDto> AsEnumerable();
            void SetVirtualMemory(Dictionary<string, int> virtualMemories);
        }

        private static IDriveSet _driveSet = null;
        public static IDriveSet DriveSet {
            get {
                if (_driveSet == null) {
                    lock (_locker) {
                        if (_driveSet == null) {
                            _driveSet = new DriveSetImpl();
                        }
                    }
                }
                return _driveSet;
            }
        }

        public class DriveSetImpl : IDriveSet {
            private const string MemoryManagementSubKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";
            private readonly List<DriveDto> _drives = new List<DriveDto>();

            public DriveSetImpl() {
                InitOnece();
            }

            // 因为虚拟内存修改后重启电脑才会生效所以不需要刷新内存中的数据
            private DateTime _diskSpaceOn = DateTime.MinValue;
            private readonly object _locker = new object();
            private void InitOnece() {
                var now = DateTime.Now;
                if (_diskSpaceOn.AddMinutes(20) < now) {
                    lock (_locker) {
                        if (_diskSpaceOn.AddMinutes(20) < now) {
                            _diskSpaceOn = now;
                            _drives.Clear();
                            var virtualMemoryDicByDriveName = GetVirtualMemoryDic();
                            foreach (var item in DriveInfo.GetDrives().Where(a => a.DriveType == DriveType.Fixed)) {
                                try {
                                    int virtualMemoryMaxSizeMb = 0;
                                    if (virtualMemoryDicByDriveName.TryGetValue(item.Name, out int value)) {
                                        virtualMemoryMaxSizeMb = value;
                                    }
                                    _drives.Add(new DriveDto(item, virtualMemoryMaxSizeMb));
                                }
                                catch (Exception e) {
                                    Logger.ErrorDebugLine(e);
                                }
                            }
                        }
                    }
                }
            }

            public int OSVirtualMemoryMb {
                get {
                    InitOnece();
                    return _drives.Sum(a => a.VirtualMemoryMaxSizeMb);
                }
            }

            public IEnumerable<DriveDto> AsEnumerable() {
                InitOnece();
                return _drives;
            }

            public string ToDiskSpaceString() {
                InitOnece();
                StringBuilder sb = new StringBuilder();
                int len = sb.Length;
                foreach (var item in _drives) {
                    if (len != sb.Length) {
                        sb.Append(";");
                    }
                    // item.Name like C:\
                    sb.Append(item.Name).Append((item.AvailableFreeSpace / NTKeyword.DoubleG).ToString("f1")).Append(" Gb");
                }
                return sb.ToString();
            }

            public void SetVirtualMemory(Dictionary<string, int> virtualMemories) {
                if (virtualMemories == null || virtualMemories.Count == 0) {
                    return;
                }
                if (virtualMemories.TryGetValue("Auto", out int virtualMemoryMb)) {
                    long virtualMemoryB = virtualMemoryMb * NTKeyword.IntM;
                    // 系统盘留出1Gb
                    long systemReserveB = NTKeyword.LongG;
                    // 非系统盘留出100Mb
                    long reserveB = 100 * NTKeyword.IntM;
                    if (_drives.Sum(a => a.AvailableFreeSpace) - systemReserveB - (_drives.Count - 1) * reserveB < virtualMemoryMb) {
                        return;
                    }
                    var systemDrive = _drives.FirstOrDefault(a => a.IsSystemDisk);
                    // 不可能没有系统盘
                    if (systemDrive == null) {
                        return;
                    }
                    int setedMb = 0;
                    List<string> list = new List<string>();
                    // 如果系统盘够大设置在系统盘
                    if (systemDrive.AvailableFreeSpace - systemReserveB > virtualMemoryB) {
                        list.Add(VirtualMemoryFormatString(systemDrive.Name, virtualMemoryMb));
                        setedMb += virtualMemoryMb;
                    }
                    else {
                        // 设置在系统盘mb
                        int mb = Convert.ToInt32((systemDrive.AvailableFreeSpace - systemReserveB) / NTKeyword.IntM);
                        list.Add(VirtualMemoryFormatString(systemDrive.Name, mb));
                        setedMb += mb;
                        var bigDrive = _drives.Where(a => !a.IsSystemDisk).OrderByDescending(a => a.AvailableFreeSpace).FirstOrDefault();
                        // 还需设置mb
                        mb = virtualMemoryMb - setedMb;
                        // 如果最大的盘可以装下剩余的虚拟内存就把剩余的都设置在这个盘
                        if (bigDrive != null && bigDrive.AvailableFreeSpace - reserveB > mb * NTKeyword.IntM) {
                            list.Add(VirtualMemoryFormatString(bigDrive.Name, mb));
                            setedMb += mb;
                        }
                        else {
                            foreach (var drive in _drives) {
                                if (drive.IsSystemDisk) {
                                    continue;
                                }
                                mb = Convert.ToInt32((drive.AvailableFreeSpace - reserveB) / NTKeyword.IntM);
                                if (mb <= 0) {
                                    continue;
                                }
                                list.Add(VirtualMemoryFormatString(drive.Name, mb));
                                setedMb += mb;
                                if (setedMb >= virtualMemoryMb) {
                                    break;
                                }
                            }
                        }
                    }
                    if (setedMb >= virtualMemoryMb) {
                        Windows.WinRegistry.SetValue(Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles", list.ToArray());
                    }
                }
                else {
                    List<string> list = new List<string>();
                    foreach (var drive in _drives) {
                        if (virtualMemories.TryGetValue(drive.Name, out int value) && value > 0) {
                            list.Add(VirtualMemoryFormatString(drive.Name, value));
                        }
                    }

                    Windows.WinRegistry.SetValue(Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles", list.ToArray());
                }
            }

            #region 静态私有方法
            private static string VirtualMemoryFormatString(string name, int value) {
                return $"{name}pagefile.sys  {value.ToString()} {value.ToString()}";
            }

            private static Dictionary<string, int> GetVirtualMemoryDic() {
                object value = Windows.WinRegistry.GetValue(Registry.LocalMachine, MemoryManagementSubKey, "PagingFiles");
                // REG_SZ or REG_MULTI_SZ
                Dictionary<string, int> dicByDriveName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                if (value is string[] vmReg) {
                    foreach (string item in vmReg) {
                        if (TryParseVirtualMemory(item, out KeyValuePair<string, int> kv)) {
                            dicByDriveName.Add(kv.Key, kv.Value);
                        }
                    }
                }
                return dicByDriveName;
            }

            private static bool TryParseVirtualMemory(string vmReg, out KeyValuePair<string, int> kv) {
                string driveName;
                try {
                    string[] strarr = vmReg.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strarr.Length == 3) {
                        driveName = strarr[0].Substring(0, 3);
                        int minsize = Convert.ToInt32(strarr[1]);
                        int maxsize = Convert.ToInt32(strarr[2]);
                        kv = new KeyValuePair<string, int>(driveName, maxsize);
                        return true;
                    }
                    kv = default;
                    return false;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    kv = default;
                    return false;
                }
            }
            #endregion
        }
    }
}
