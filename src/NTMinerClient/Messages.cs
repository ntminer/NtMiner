using NTMiner.Hub;
using System;

namespace NTMiner {
    [MessageType(description: "打开内核下载界面")]
    public class ShowKernelDownloaderCommand : Cmd {
        public ShowKernelDownloaderCommand(Guid kernelId, Action<bool, string> downloadComplete) {
            this.KernelId = kernelId;
            this.DownloadComplete = downloadComplete;
        }

        public Guid KernelId { get; private set; }
        public Action<bool, string> DownloadComplete { get; private set; }
    }

    [MessageType(description: "设置开机自动挖矿")]
    public class SetAutoStartCommand : Cmd {
        public SetAutoStartCommand(bool isAutoBoot, bool isAutoStart) {
            this.IsAutoBoot = isAutoBoot;
            this.IsAutoStart = isAutoStart;
        }

        public bool IsAutoBoot { get; private set; }
        public bool IsAutoStart { get; private set; }
    }

    [MessageType(description: "ServerContext刷新后")]
    public class ServerContextReInitedEvent : EventBase {
        public ServerContextReInitedEvent() { }
    }

    [MessageType(description: "ServerContext的视图层刷新完毕后")]
    public class ServerContextVmsReInitedEvent : EventBase {
        public ServerContextVmsReInitedEvent() { }
    }

    [MessageType(description: "LocalContext刷新后")]
    public class LocalContextReInitedEvent : EventBase {
        public LocalContextReInitedEvent() { }
    }

    [MessageType(description: "LocalContext的视图层刷新完毕后")]
    public class LocalContextVmsReInitedEvent : EventBase {
        public LocalContextVmsReInitedEvent() { }
    }

    [MessageType(description: "发现了NTMiner或MinerStudio新版本")]
    public class AppVersionChangedEvent : EventBase {
        /// <summary>
        /// 如果给定的服务器版本比本地版本高则发布AppVersionChangedEvent事件
        /// </summary>
        /// <param name="serverVersion"></param>
        public static void PublishIfNewVersion(string serverVersion) {
            if (!string.IsNullOrEmpty(serverVersion) && serverVersion != MainAssemblyInfo.CurrentVersion.ToString()) {
                if (Version.TryParse(serverVersion, out Version v)) {
                    if (v > MainAssemblyInfo.CurrentVersion) {
                        NTMinerRoot.ServerVersion = v;
                        VirtualRoot.RaiseEvent(new AppVersionChangedEvent());
                    }
                }
            }
        }

        public AppVersionChangedEvent() { }
    }

    [MessageType(description: "内核自我重启")]
    public class KernelSelfRestartedEvent : EventBase {
        public KernelSelfRestartedEvent() { }
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

    [MessageType(description: "挖矿上下文发生了变更")]
    public class CurrentMineContextChangedEvent : EventBase {
        public CurrentMineContextChangedEvent() { }
    }

    [MessageType(description: "挖矿停止事件")]
    public class MineStopedEvent : EventBase {
        public MineStopedEvent(IMineContext mineContext, StopMineReason stopReason) {
            this.MineContext = mineContext;
            this.StopReason = stopReason;
        }
        public IMineContext MineContext { get; private set; }
        public StopMineReason StopReason { get; private set; }
    }

    [MessageType(description: "关闭NTMiner客户端")]
    // ReSharper disable once InconsistentNaming
    public class CloseNTMinerCommand : Cmd {
        public CloseNTMinerCommand(string reason) {
            this.Reason = reason;
        }

        public string Reason { get; private set; }
    }

    [MessageType(description: "刷新开机启动和自动挖矿")]
    public class RefreshAutoBootStartCommand : Cmd {
        public RefreshAutoBootStartCommand() { }
    }
}
