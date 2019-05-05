using NTMiner.Bus;
using NTMiner.Core;

namespace NTMiner {
    [MessageType(description: "ServerContext刷新后")]
    public class ServerContextReInitedEvent : EventBase {
        public ServerContextReInitedEvent() { }
    }

    [MessageType(description: "ServerContext的视图层刷新完毕后")]
    public class ServerContextVmsReInitedEvent : EventBase {
        public ServerContextVmsReInitedEvent() { }
    }

    [MessageType(description: "MinerProfile刷新后")]
    public class MinerProfileReInitedEvent : EventBase {
        public MinerProfileReInitedEvent() { }
    }

    [MessageType(description: "主界面显示后")]
    public class MainWindowShowedEvent : EventBase {
        public MainWindowShowedEvent() { }
    }

    [MessageType(description: "发现了服务端新版本")]
    public class ServerVersionChangedEvent : EventBase {
        public ServerVersionChangedEvent() { }
    }

    [MessageType(description: "开始挖矿不成功")]
    public class StartingMineFailedEvent : EventBase {
        public StartingMineFailedEvent(string message) {
            this.Message = message;
        }

        public string Message { get; private set; }
    }

    [MessageType(description: "挖矿开始事件")]
    public class MineStartedEvent : EventBase {
        public MineStartedEvent(IMineContext mineContext) {
            this.MineContext = mineContext;
        }
        public IMineContext MineContext { get; private set; }
    }

    [MessageType(description: "挖矿停止事件")]
    public class MineStopedEvent : EventBase {
        public MineStopedEvent(IMineContext mineContext) {
            this.MineContext = mineContext;
        }
        public IMineContext MineContext { get; private set; }
    }

    [MessageType(description: "关闭NTMiner客户端")]
    // ReSharper disable once InconsistentNaming
    public class CloseNTMinerCommand : Cmd {
        public CloseNTMinerCommand() { }
    }

    [MessageType(description: "关闭主界面")]
    public class CloseMainWindowCommand : Cmd {
        public CloseMainWindowCommand() { }
    }

    [MessageType(description: "刷新开机启动和自动挖矿")]
    public class RefreshAutoBootStartCommand : Cmd {
        public RefreshAutoBootStartCommand() { }
    }
}
