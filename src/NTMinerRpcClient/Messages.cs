using NTMiner.Hub;

namespace NTMiner {

    [MessageType(description: "切换了群控后台客户端服务类型后")]
    public class MinerStudioServiceSwitchedEvent : EventBase {
        public MinerStudioServiceSwitchedEvent(MinerStudioServiceType serviceType) {
            this.ServiceType = serviceType;
        }

        public MinerStudioServiceType ServiceType { get; private set; }
    }

    [MessageType(description: "刷新开源矿工程序版本文件集")]
    public class RefreshNTMinerFileSetCommand : Cmd {
        public RefreshNTMinerFileSetCommand() { }
    }

    [MessageType(description: "开源矿工程序版本文件集初始化后")]
    public class NTMinerFileSetInitedEvent : EventBase {
        public NTMinerFileSetInitedEvent() { }
    }
}
