using NTMiner.Hub;

namespace NTMiner {

    [MessageType(description: "切换了群控后台客户端服务类型后")]
    public class MinerStudioServiceSwitchedEvent : EventBase {
        public MinerStudioServiceSwitchedEvent(MinerStudioServiceType serviceType) {
            this.ServiceType = serviceType;
        }

        public MinerStudioServiceType ServiceType { get; private set; }
    }
}
