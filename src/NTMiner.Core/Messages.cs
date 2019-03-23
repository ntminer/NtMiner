using NTMiner.Bus;
using NTMiner.Core;

namespace NTMiner {
    [MessageType(messageType: typeof(MineStartedEvent), description: "挖矿开始事件")]
    public class MineStartedEvent : EventBase {
        public MineStartedEvent(IMineContext mineContext) {
            this.MineContext = mineContext;
        }
        public IMineContext MineContext { get; private set; }
    }

    [MessageType(messageType: typeof(MineStartedEvent), description: "挖矿停止事件")]
    public class MineStopedEvent : EventBase {
        public MineStopedEvent(IMineContext mineContext) {
            this.MineContext = mineContext;
        }
        public IMineContext MineContext { get; private set; }
    }

    [MessageType(messageType: typeof(ShowMainWindowCommand), description: "显式主界面")]
    public class ShowMainWindowCommand : Cmd {
    }

    [MessageType(messageType: typeof(CloseNTMinerCommand), description: "关闭NTMiner客户端")]
    public class CloseNTMinerCommand : Cmd {
    }

    [MessageType(messageType: typeof(RefreshAutoBootStartCommand), description: "刷新开机自动启动和启动后自动开始挖矿")]
    public class RefreshAutoBootStartCommand : Cmd {
    }
}
