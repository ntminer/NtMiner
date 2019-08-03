using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using System;
using System.Diagnostics;
using System.Threading;

namespace NTMiner {
    public partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static bool IsMinerStudio = false;
        public static string LocalDirFullName { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");

        public static bool IsMinerClientRunning {
            get {
                try {
                    Mutex.OpenExisting("ntminerclient");
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        public static readonly IMessageDispatcher SMessageDispatcher;
        private static readonly ICmdBus SCommandBus;
        private static readonly IEventBus SEventBus;

        static VirtualRoot() {
            SMessageDispatcher = new MessageDispatcher();
            SCommandBus = new DirectCommandBus(SMessageDispatcher);
            SEventBus = new DirectEventBus(SMessageDispatcher);
        }
    }
}
