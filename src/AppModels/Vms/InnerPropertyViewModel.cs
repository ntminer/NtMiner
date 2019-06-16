using System;

namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        public InnerPropertyViewModel() {
        }

        public Guid Id {
            get { return VirtualRoot.Id; }
        }
        public DateTime BootOn {
            get => NTMinerRoot.Instance.CreatedOn;
        }
        public string GlobalDir {
            get => AssemblyInfo.GlobalDirFullName;
        }
        public string ServerDbFileFullName {
            get => SpecialPath.ServerDbFileFullName;
        }
        public string LocalDbFileFullName {
            get => SpecialPath.LocalDbFileFullName;
        }

        public string ServerJsonFileFullName {
            get { return SpecialPath.ServerJsonFileFullName; }
        }

        public string ServerVersionJsonFileFullName {
            get { return AssemblyInfo.ServerVersionJsonFileFullName; }
        }

        public string DaemonFileFullName {
            get { return SpecialPath.DaemonFileFullName; }
        }

        public string DevConsoleFileFullName {
            get { return SpecialPath.DevConsoleFileFullName; }
        }

        public string NTMinerOverClockFileFullName {
            get { return SpecialPath.NTMinerOverClockFileFullName; }
        }

        public string TempDirFullName {
            get { return SpecialPath.TempDirFullName; }
        }

        public string PackagesDirFullName {
            get { return SpecialPath.PackagesDirFullName; }
        }

        public string DownloadDirFullName {
            get {
                return SpecialPath.DownloadDirFullName;
            }
        }

        public string KernelsDirFullName {
            get { return SpecialPath.KernelsDirFullName; }
        }

        public string LogsDirFullName {
            get { return SpecialPath.LogsDirFullName; }
        }

        public bool IsMinerStudio {
            get {
                return NTMinerRoot.IsUseDevConsole;
            }
        }
    }
}
