using NTMiner.Hub;

namespace NTMiner {
    [MessageType(description: "windows注销或关闭时")]
    public class WindowsSessionEndingEvent : EventBase {
        public enum ReasonSessionEnding {
            Logoff,
            Shutdown,
            Unknown
        }

        public WindowsSessionEndingEvent(ReasonSessionEnding sessionEndingReason) {
            this.SessionEndingReason = sessionEndingReason;
        }

        public ReasonSessionEnding SessionEndingReason { get; private set; }
    }
}
