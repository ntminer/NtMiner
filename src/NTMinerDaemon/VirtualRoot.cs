using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using System.Diagnostics;

namespace NTMiner {
    public partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static bool IsMinerStudio = false;

        public static readonly IMessageDispatcher MessageDispatcher;
        private static readonly ICmdBus _commandBus;
        private static readonly IEventBus _eventBus;

        static VirtualRoot() {
            MessageDispatcher = new MessageDispatcher();
            _commandBus = new DirectCommandBus(MessageDispatcher);
            _eventBus = new DirectEventBus(MessageDispatcher);
        }
    }
}
