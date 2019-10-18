using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using System.Diagnostics;

namespace NTMiner {
    public partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static bool IsMinerStudio = false;

        public static readonly IMessageDispatcher SMessageDispatcher;
        private static readonly ICmdBus SCommandBus;
        private static readonly IEventBus SEventBus;
        public static readonly WorkerEventSet WorkerEvents;

        static VirtualRoot() {
            SMessageDispatcher = new MessageDispatcher();
            SCommandBus = new DirectCommandBus(SMessageDispatcher);
            SEventBus = new DirectEventBus(SMessageDispatcher);
            WorkerEvents = new WorkerEventSet();
        }
    }
}
