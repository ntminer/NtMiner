using NTMiner.Core.Mq.Senders;
using NTMiner.ServerNode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class WsServerNodeSet : IWsServerNodeSet {
        public class WsServerNodeActiveOn : WsServerNodeState, IEntity<string> {
            public static WsServerNodeActiveOn Create(WsServerNodeState data) {
                return new WsServerNodeActiveOn {
                    Address = data.Address,
                    Description = data.Description,
                    MinerClientWsSessionCount = data.MinerClientWsSessionCount,
                    MinerStudioWsSessionCount = data.MinerStudioWsSessionCount,
                    MinerClientSessionCount = data.MinerClientSessionCount,
                    MinerStudioSessionCount = data.MinerStudioSessionCount,
                    AvailablePhysicalMemory = data.AvailablePhysicalMemory,
                    Cpu = data.Cpu,
                    CpuPerformance = data.CpuPerformance,
                    CpuTemperature = data.CpuTemperature,
                    OSInfo = data.OSInfo,
                    TotalPhysicalMemory = data.TotalPhysicalMemory,
                    ActiveOn = DateTime.Now,
                };
            }

            public string GetId() {
                return this.Address;
            }

            public DateTime ActiveOn { get; set; }
        }

        private readonly Dictionary<string, WsServerNodeActiveOn> _dicByIp = new Dictionary<string, WsServerNodeActiveOn>();

        private ShardingHasher _consistentHash = ShardingHasher.Empty;

        private void ReSetConsistentHash() {
            _consistentHash = new ShardingHasher(_dicByIp.Keys.ToArray());
        }

        private readonly IWsServerNodeMqSender _mqSender;
        public WsServerNodeSet(IWsServerNodeMqSender mqSender) {
            _mqSender = mqSender;
            VirtualRoot.AddEventPath<Per20SecondEvent>("周期将不活跃的节点移除", LogEnum.DevConsole, action: message => {
                DateTime activeOn = DateTime.Now.AddSeconds(-message.Seconds);
                var toRemoves = _dicByIp.Values.Where(a => a.ActiveOn < activeOn).ToArray();
                foreach (var item in toRemoves) {
                    this.RemoveNode(item.Address);
                }
                if (toRemoves.Length != 0) {
                    Write.UserInfo($"移除了 {toRemoves.Length.ToString()} 个节点");
                }
            }, this.GetType());
        }

        public WsStatus WsStatus {
            get {
                return _dicByIp.Count != 0 ? WsStatus.Online : WsStatus.Offline;
            }
        }

        public string GetTargetNode(Guid clientId) {
            return _consistentHash.GetTargetNode(clientId);
        }

        public void SetNodeState(WsServerNodeState data) {
            if (data == null || string.IsNullOrEmpty(data.Address)) {
                return;
            }
            if (!data.IsAddressValid()) {
                return;
            }
            if (_dicByIp.TryGetValue(data.Address, out WsServerNodeActiveOn nodeData)) {
                nodeData.Update(data);
            }
            else {
                nodeData = WsServerNodeActiveOn.Create(data);
                _dicByIp.Add(data.Address, nodeData);
                ReSetConsistentHash();
                _mqSender.SendWsServerNodeAdded(data.Address);
            }
        }

        public void RemoveNode(string address) {
            if (string.IsNullOrEmpty(address)) {
                return;
            }
            _dicByIp.Remove(address);
            ReSetConsistentHash();
            _mqSender.SendWsServerNodeRemoved(address);
        }

        public IEnumerable<WsServerNodeState> AsEnumerable() {
            return _dicByIp.Values;
        }
    }
}
