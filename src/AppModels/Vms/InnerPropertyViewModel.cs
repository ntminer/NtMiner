using System;

namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        public InnerPropertyViewModel() {
        }

        public Guid Id {
            get { return VirtualRoot.Id; }
        }
        public string BootOn {
            get => NTMinerRoot.Instance.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string LocalDir {
            get => AssemblyInfo.LocalDirFullName;
        }
        public string ServerDbFileFullName {
            get {
                return SpecialPath.ServerDbFileFullName.Replace(LocalDir, Consts.LocalDirParameterName);
            }
        }
        public string LocalDbFileFullName {
            get => SpecialPath.LocalDbFileFullName.Replace(LocalDir, Consts.LocalDirParameterName);
        }

        public string ServerJsonFileFullName {
            get { return SpecialPath.ServerJsonFileFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string ServerVersionJsonFileFullName {
            get { return AssemblyInfo.ServerVersionJsonFileFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string DaemonFileFullName {
            get { return SpecialPath.DaemonFileFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string DevConsoleFileFullName {
            get { return SpecialPath.DevConsoleFileFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string NTMinerOverClockFileFullName {
            get { return SpecialPath.NTMinerOverClockFileFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string TempDirFullName {
            get { return SpecialPath.TempDirFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string PackagesDirFullName {
            get { return SpecialPath.PackagesDirFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string DownloadDirFullName {
            get {
                return SpecialPath.DownloadDirFullName.Replace(LocalDir, Consts.LocalDirParameterName);
            }
        }

        public string KernelsDirFullName {
            get { return SpecialPath.KernelsDirFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string LogsDirFullName {
            get { return SpecialPath.LogsDirFullName.Replace(LocalDir, Consts.LocalDirParameterName); }
        }

        public string AppRuntime {
            get {
                if (VirtualRoot.IsMinerStudio) {
                    return "群控客户端";
                }
                else if (VirtualRoot.IsMinerClient) {
                    return "挖矿端";
                }
                return "未知";
            }
        }
    }
}
