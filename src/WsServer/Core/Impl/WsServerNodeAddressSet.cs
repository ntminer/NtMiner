using NTMiner.Core.Mq.Senders;
using NTMiner.Core.Redis;
using NTMiner.ServerNode;
using System;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class WsServerNodeAddressSet : WsServerNodeAddressSetBase, IWsServerNodeAddressSet {
        private readonly IWsServerNodeRedis _wsServerNodeRedis;
        private readonly IWsServerNodeMqSender _wsServerNodeMqSender;
        public WsServerNodeAddressSet(IWsServerNodeRedis wsServerNodeRedis, IWsServerNodeMqSender wsServerNodeMqSender) : base(wsServerNodeRedis) {
            _wsServerNodeRedis = wsServerNodeRedis;
            _wsServerNodeMqSender = wsServerNodeMqSender;
            VirtualRoot.BuildOnecePath<WebSocketServerStatedEvent>("上报节点信息，获取节点列表", LogEnum.UserConsole, path: _ => {
                ReportNodeAsync(callback: () => {
                    base.Init(callback: () => {
                        NTMinerConsole.UserOk("Ws服务器节点地址集初始化完成");
                        VirtualRoot.RaiseEvent(new WsServerNodeAddressSetInitedEvent());
                    });
                    _wsServerNodeMqSender.SendWsServerNodeAdded(ServerRoot.HostConfig.ThisServerAddress);
                });
                VirtualRoot.BuildEventPath<Per10SecondEvent>("节点呼吸", LogEnum.UserConsole, path: message => {
                    ReportNodeAsync();
                }, this.GetType());
            }, PathId.Empty, this.GetType());
        }

        private void ReportNodeAsync(Action callback = null) {
            Task.Factory.StartNew(() => {
                WsServerNodeState nodeState = null;
                try {
                    int minerClientWsSessionCount = 0;
                    int minerStudioWsSessionCount = 0;
                    minerClientWsSessionCount = AppRoot.WsServer.MinerClientWsSessions.Count;
                    minerStudioWsSessionCount = AppRoot.WsServer.MinerStudioWsSessions.Count;
                    var ram = Windows.Ram.Instance;
                    var cpu = Windows.Cpu.Instance;
                    nodeState = new WsServerNodeState {
                        Address = ServerRoot.HostConfig.ThisServerAddress,
                        Description = string.Empty,
                        MinerClientSessionCount = AppRoot.MinerClientSessionSet.Count,
                        MinerStudioSessionCount = AppRoot.MinerStudioSessionSet.Count,
                        MinerClientWsSessionCount = minerClientWsSessionCount,
                        MinerStudioWsSessionCount = minerStudioWsSessionCount,
                        Cpu = cpu.ToData(),
                        TotalPhysicalMemory = ram.TotalPhysicalMemory,
                        AvailablePhysicalMemory = ram.AvailablePhysicalMemory,
                        OSInfo = Windows.OS.Instance.OsInfo,
                        CpuPerformance = cpu.GetTotalCpuUsage(),
                        // 以下三个属性的访问约耗时30毫秒所以放在Task中
                        ProcessMemoryMb = VirtualRoot.ProcessMemoryMb,
                        ThreadCount = VirtualRoot.ThreadCount,
                        HandleCount = VirtualRoot.HandleCount,
                        AvailableFreeSpaceInfo = VirtualRoot.GetAvailableFreeSpaceInfo()
                    };
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                _wsServerNodeRedis.SetAsync(nodeState).ContinueWith(t => {
                    if (t.Exception != null) {
                        NTMinerConsole.UserFail("呼吸失败：" + t.Exception.Message);
                    }
                    else {
                        NTMinerConsole.UserOk("呼吸成功");
                    }
                    callback?.Invoke();
                });
            });
        }
    }
}
