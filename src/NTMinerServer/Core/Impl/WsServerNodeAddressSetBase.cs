using NTMiner.Core.Redis;
using NTMiner.ServerNode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class WsServerNodeAddressSetBase : IWsServerNodeAddressSet {
        private readonly IReadOnlyWsServerNodeRedis _wsServerNodeRedis;
        private List<string> _addressList = new List<string>();
        private ShardingHasher _consistentHash = ShardingHasher.Empty;

        public WsServerNodeAddressSetBase(IReadOnlyWsServerNodeRedis wsServerNodeRedis) {
            _wsServerNodeRedis = wsServerNodeRedis;
            VirtualRoot.AddEventPath<Per20SecondEvent>("周期从redis中刷新", LogEnum.DevConsole, action: message => {
                Init();
            }, this.GetType());
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在WsServerNodeAddressSetInitedEvent事件之后才会创建
            VirtualRoot.AddEventPath<WsServerNodeRemovedMqMessage>("收到移除了服务器节点Mq消息后敲响打扫时间到的铃声", LogEnum.UserConsole, action: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                Init();
            }, this.GetType());
            VirtualRoot.AddEventPath<WsServerNodeAddedMqMessage>("收到添加了服务器节点Mq消息后敲响打扫时间到的铃声", LogEnum.UserConsole, action: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                Init();
            }, this.GetType());
            Init();
        }

        public void Init(Action callback = null) {
            _wsServerNodeRedis.GetAllAddress().ContinueWith(t => {
                _addressList = t.Result.Keys.ToList();
                var data = _addressList.ToArray();
                _consistentHash = new ShardingHasher(data);
                callback?.Invoke();
            });
        }

        public WsStatus WsStatus {
            get {
                return _addressList.Count != 0 ? WsStatus.Online : WsStatus.Offline;
            }
        }

        public string GetTargetNode(Guid clientId) {
            return _consistentHash.GetTargetNode(clientId);
        }

        public IEnumerable<string> AsEnumerable() {
            return _addressList;
        }
    }
}
