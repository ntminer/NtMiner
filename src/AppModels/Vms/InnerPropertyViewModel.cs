using System;

namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        const string GlobalDirParameterName = "{全局目录}";
        public InnerPropertyViewModel() {
        }

        public Guid Id {
            get { return VirtualRoot.Id; }
        }
        public string BootOn {
            get => NTMinerRoot.Instance.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string GlobalDir {
            get => AssemblyInfo.LocalDirFullName;
        }
        public string ServerDbFileFullName {
            get {
                return SpecialPath.ServerDbFileFullName.Replace(GlobalDir, GlobalDirParameterName);
            }
        }
        public string LocalDbFileFullName {
            get => SpecialPath.LocalDbFileFullName.Replace(GlobalDir, GlobalDirParameterName);
        }

        public string ServerJsonFileFullName {
            get { return SpecialPath.ServerJsonFileFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string ServerVersionJsonFileFullName {
            get { return AssemblyInfo.ServerVersionJsonFileFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string DaemonFileFullName {
            get { return SpecialPath.DaemonFileFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string DevConsoleFileFullName {
            get { return SpecialPath.DevConsoleFileFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string NTMinerOverClockFileFullName {
            get { return SpecialPath.NTMinerOverClockFileFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string TempDirFullName {
            get { return SpecialPath.TempDirFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string PackagesDirFullName {
            get { return SpecialPath.PackagesDirFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string DownloadDirFullName {
            get {
                return SpecialPath.DownloadDirFullName.Replace(GlobalDir, GlobalDirParameterName);
            }
        }

        public string KernelsDirFullName {
            get { return SpecialPath.KernelsDirFullName.Replace(GlobalDir, GlobalDirParameterName); }
        }

        public string LogsDirFullName {
            get { return SpecialPath.LogsDirFullName.Replace(GlobalDir, GlobalDirParameterName); }
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
