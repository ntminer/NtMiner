using NTMiner.Hub;

namespace NTMiner {
    [MessageType(description: "程序退出时")]
    public class AppExitEvent : EventBase {
        public AppExitEvent() { }
    }

}
