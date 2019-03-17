using NTMiner.Bus;
using NTMiner.Core;
using System;

namespace NTMiner {
    [MessageType(messageType: typeof(ReInitMinerProfileCommand), description: "重新初始化MinerProfile命令")]
    public class ReInitMinerProfileCommand : Cmd {
        public ReInitMinerProfileCommand() {
        }
    }

    [MessageType(messageType: typeof(MinerProfileReInitedEvent), description: "MinerProfile重新初始化后事件")]
    public class MinerProfileReInitedEvent : EventBase {
        public MinerProfileReInitedEvent() {
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
