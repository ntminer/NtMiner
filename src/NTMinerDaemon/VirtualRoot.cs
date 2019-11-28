using NTMiner.Bus;
using System.Diagnostics;

namespace NTMiner {
    public partial class VirtualRoot {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static bool IsMinerStudio = false;

        public static readonly IMessageDispatcher MessageDispatcher;

        static VirtualRoot() {
            MessageDispatcher = new MessageDispatcher();
        }
    }
}
