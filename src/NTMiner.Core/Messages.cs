using NTMiner.Bus;
using NTMiner.Core;
using System;

namespace NTMiner {
    [MessageType(description: "打开内核列表窗口")]
    public class ShowKernelsWindowCommand : Cmd {
        public ShowKernelsWindowCommand() {
        }
    }

    [MessageType(description: "打开内核下载界面")]
    public class ShowKernelDownloaderCommand : Cmd {
        public ShowKernelDownloaderCommand(Guid kernelId, Action<bool, string> downloadComplete) {
            this.KernelId = kernelId;
            this.DownloadComplete = downloadComplete;
        }

        public Guid KernelId { get; private set; }
        public Action<bool, string> DownloadComplete { get; private set; }
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

    [MessageType(description: "主界面显示后")]
    public class MainWindowShowedEvent : EventBase {
        public MainWindowShowedEvent() { }
    }

    [MessageType(description: "发现了NTMiner或MinerStudio新版本")]
    public class AppVersionChangedEvent : EventBase {
        /// <summary>
        /// 如果给定的服务器版本比本地版本高则发布AppVersionChangedEvent事件
        /// </summary>
        /// <param name="serverVersion"></param>
        public static void PublishIfNewVersion(string serverVersion) {
            if (!string.IsNullOrEmpty(serverVersion) && serverVersion != NTMinerRoot.CurrentVersion.ToString()) {
                NTMinerRoot.ServerVersion = serverVersion;
                VirtualRoot.Happened(new AppVersionChangedEvent());
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
        public CloseMainWindowCommand(string message) {
            this.Message = message;
        }

        public string Message { get; private set; }
    }

    [MessageType(description: "刷新开机启动和自动挖矿")]
    public class RefreshAutoBootStartCommand : Cmd {
        public RefreshAutoBootStartCommand() { }
    }
}
