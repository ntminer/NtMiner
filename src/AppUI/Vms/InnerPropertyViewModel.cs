using System;

namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        public InnerPropertyViewModel() {
        }

        public string AppName {
            get => NTMinerRoot.AppName;
        }
        public Guid Id {
            get { return ClientId.Id; }
        }
        public DateTime BootOn {
            get => NTMinerRoot.Current.CreatedOn;
        }
        public LangViewModels LangVms {
            get => LangViewModels.Current;
        }
        public string GlobalDir {
            get => VirtualRoot.GlobalDirFullName;
        }
        public string LangDbFileFullName {
            get => ClientId.LangDbFileFullName;
        }
        public string LocalLangJsonFileFullName {
            get => ClientId.LocalLangJsonFileFullName;
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

        public string LangVersionJsonFileFullName {
            get { return AssemblyInfo.LangVersionJsonFileFullName; }
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
