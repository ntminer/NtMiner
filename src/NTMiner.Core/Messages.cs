using NTMiner.Bus;
using NTMiner.Core;
using System;

namespace NTMiner {
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

    [MessageType(messageType: typeof(RestartNTMinerCommand), description: "重启NTMiner")]
    public class RestartNTMinerCommand : Cmd {
        public RestartNTMinerCommand(bool isWorkEdit, Guid mineWorkId) {
            this.MineWorkId = mineWorkId;
            this.IsWorkEdit = isWorkEdit;
        }
        public bool IsWorkEdit { get; private set; }
        public Guid MineWorkId { get; private set; }
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
}
