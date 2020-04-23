using NTMiner.ServerNode;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp.Server;

namespace NTMiner.Core.Impl {
    public class WsServerNodeAddressSet : IWsServerNodeAddressSet {
        private string[] _nodeAddresses = new string[0];
        public WsServerNodeAddressSet() {
            VirtualRoot.AddOnecePath<WebSocketServerStatedEvent>("WebSocket服务启动后上报节点信息，获取节点列表", LogEnum.UserConsole, action: _ => {
                ReportNodeAsync(callback: () => {
                    RefreshNodes(callback: () => {
                        Write.UserOk("Ws服务器节点地址集初始化完成");
                        VirtualRoot.RaiseEvent(new WsServerNodeAddressSetInitedEvent());
                    });
                });
                VirtualRoot.AddEventPath<Per10SecondEvent>("节点呼吸", LogEnum.UserConsole, action: message => {
                    ReportNodeAsync();
                }, this.GetType());
                VirtualRoot.AddEventPath<Per1MinuteEvent>("敲响打扫时间到的铃声", LogEnum.UserConsole, action: message => {
                    RefreshNodes();
                }, typeof(WsRoot));
                // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在WsServerNodeAddressSetInitedEvent事件之后才会创建
                VirtualRoot.AddEventPath<WsServerNodeRemovedMqMessage>("收到移除了服务器节点Mq消息后敲响打扫时间到的铃声", LogEnum.UserConsole, action: message => {
                    if (message.WsServerAddress == ServerRoot.HostConfig.ThisServerAddress) {
                        return;
                    }
                    RefreshNodes();
                }, typeof(WsRoot));
                VirtualRoot.AddEventPath<WsServerNodeAddedMqMessage>("收到添加了服务器节点Mq消息后敲响打扫时间到的铃声", LogEnum.UserConsole, action: message => {
                    if (message.WsServerAddress == ServerRoot.HostConfig.ThisServerAddress) {
                        return;
                    }
                    RefreshNodes();
                }, typeof(WsRoot));
            }, PathId.Empty, this.GetType());
        }

        private void RefreshNodes(Action callback = null) {
            RpcRoot.OfficialServer.WsServerNodeService.GetNodeAddressesAsync((response, e) => {
                if (response.IsSuccess()) {
                    _nodeAddresses = response.Data;
                    VirtualRoot.RaiseEvent(new CleanTimeArrivedEvent(GetNodeAddresses(response.Data)));
                }
                else {
                    Write.UserError("获取节点列表失败：" + response.ReadMessage(e));
                }
                callback?.Invoke();
            });
        }

        private string[] GetNodeAddresses(string[] nodeAddresses) {
            if (nodeAddresses == null || nodeAddresses.Length == 0) {
                Write.UserWarn("节点集为空");
                return new string[0];
            }
            string thisServerAddress = ServerRoot.HostConfig.ThisServerAddress;
            var thisNode = nodeAddresses.FirstOrDefault(a => a == thisServerAddress);
            if (thisNode == null) {
                Write.UserWarn($"未发现和本节点地址相同的节点，本节点地址为：{(string.IsNullOrEmpty(thisServerAddress) ? "无" : thisServerAddress)}");
            }
            return nodeAddresses;
        }

        private void ReportNodeAsync(Action callback = null) {
            int minerClientWsSessionCount = 0;
            int minerStudioWsSessionCount = 0;
            if (WsRoot.MinerClientSessionSet.TryGetWsSessions(out WebSocketSessionManager wsSessionManager)) {
                minerClientWsSessionCount = wsSessionManager.Count;
            }
            if (WsRoot.MinerStudioSessionSet.TryGetWsSessions(out wsSessionManager)) {
                minerStudioWsSessionCount = wsSessionManager.Count;
            }
            var ram = Windows.Ram.Instance;
            var cpu = Windows.Cpu.Instance;
            cpu.GetSensorValue(out double performance, out float temperature, out double _);
            RpcRoot.OfficialServer.WsServerNodeService.ReportNodeStateAsync(new WsServerNodeState {
                Address = ServerRoot.HostConfig.ThisServerAddress,
                Description = string.Empty,
                MinerClientSessionCount = WsRoot.MinerClientSessionSet.Count,
                MinerStudioSessionCount = WsRoot.MinerStudioSessionSet.Count,
                MinerClientWsSessionCount = minerClientWsSessionCount,
                MinerStudioWsSessionCount = minerStudioWsSessionCount,
                Cpu = cpu.ToData(),
                TotalPhysicalMemory = ram.TotalPhysicalMemory,
                AvailablePhysicalMemory = ram.AvailablePhysicalMemory,
                OSInfo = Windows.OS.Instance.OsInfo,
                CpuPerformance = performance,
                CpuTemperature = temperature
            }, (response, e) => {
                if (response.IsSuccess()) {
                    Write.UserOk("呼吸成功");
                }
                else {
                    Write.UserFail("呼吸失败：" + response.ReadMessage(e));
                }
                callback?.Invoke();
            });
        }

        public IEnumerable<string> AsEnumerable() {
            return _nodeAddresses;
        }
    }
}
