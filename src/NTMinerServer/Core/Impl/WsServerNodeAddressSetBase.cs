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
            VirtualRoot.BuildEventPath<Per20SecondEvent>("周期从redis中刷新WsServer节点集", LogEnum.DevConsole, path: message => {
                Init();
            }, this.GetType());
            // 收到Mq消息之前一定已经初始化完成，因为Mq消费者在WsServerNodeAddressSetInitedEvent事件之后才会创建
            VirtualRoot.BuildEventPath<WsServerNodeRemovedMqEvent>("收到移除了服务器节点Mq消息后刷新节点列表", LogEnum.UserConsole, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                Init();
            }, this.GetType());
            VirtualRoot.BuildEventPath<WsServerNodeAddedMqEvent>("收到添加了服务器节点Mq消息后刷新节点列表", LogEnum.UserConsole, path: message => {
                if (message.AppId == ServerRoot.HostConfig.ThisServerAddress) {
                    return;
                }
                Init();
            }, this.GetType());
            Init();
        }

        public void Init(Action callback = null) {
            _wsServerNodeRedis.GetAllAddress().ContinueWith(t => {
                var now = DateTime.Now;
                _addressList = t.Result.Where(a => !IsOffline(a.Value, now)).Select(a => a.Key).ToList();
                var data = _addressList.ToArray();
                _consistentHash = new ShardingHasher(data);
                callback?.Invoke();
            });
        }

        protected static bool IsOffline(DateTime activeOn, DateTime now) {
            // 如果WsServer节点服务器的时间和WebApiServer的时间相差很多则该WsServer会被下线
            return activeOn.AddSeconds(30) < now;
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
