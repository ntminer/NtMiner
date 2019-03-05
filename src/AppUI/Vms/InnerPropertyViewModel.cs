using System;
using System.IO;

namespace NTMiner.Vms {
    public class InnerPropertyViewModel : ViewModelBase {
        public InnerPropertyViewModel() {
        }

        public string AppName {
            get => NTMinerRoot.AppName;
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
        public string ServerDbFileFullName {
            get => Path.Combine(VirtualRoot.GlobalDirFullName, "server.litedb");
        }
        public string LocalDbFileFullName {
            get => Path.Combine(VirtualRoot.GlobalDirFullName, "local.litedb");
        }

        public bool IsControlCenter {
            get {
                return NTMinerRoot.IsUseDevConsole;
            }
        }
    }
}
