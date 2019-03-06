using NTMiner.Bus;
using NTMiner.Core;
using System;

namespace NTMiner {
    [MessageType(messageType: typeof(HotKeyChangedEvent), description: "热键切换后事件")]
    public class HotKeyChangedEvent : EventBase {
        public HotKeyChangedEvent() {
        }
    }

    [MessageType(messageType: typeof(SwichMinerProfileCommand), description: "切换MinerProfile命令")]
    public class SwichMinerProfileCommand : Cmd {
        public SwichMinerProfileCommand(Guid workId) {
            this.WorkId = workId;
        }

        public Guid WorkId { get; private set; }
    }

    [MessageType(messageType: typeof(MinerProfileSwichedEvent), description: "MinerProfile切换后事件")]
    public class MinerProfileSwichedEvent : EventBase {
        public MinerProfileSwichedEvent() {
        }
    }

    [MessageType(messageType: typeof(RefreshArgsAssemblyCommand), description: "刷新参数总成命令")]
    public class RefreshArgsAssemblyCommand : Cmd {
        public RefreshArgsAssemblyCommand() { }
    }

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
}
