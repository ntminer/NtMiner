using NTMiner.Core.Redis;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class WsServerNodeAddressSet : WsServerNodeAddressSetBase, IWsServerNodeAddressSet {
        public WsServerNodeAddressSet(IWsServerNodeRedis wsServerNodeRedis) : base(wsServerNodeRedis) {
            VirtualRoot.BuildEventPath<Per10SecondEvent>("清理掉离线的WsServer节点", LogEnum.None, path: message => {
                wsServerNodeRedis.GetAllAddress().ContinueWith(t => {
                    var offlines = GetOfflineAddress(t.Result);
                    if (offlines != null && offlines.Count != 0) {
                        wsServerNodeRedis.ClearAsync(offlines).ContinueWith(_=> {
                            NTMinerConsole.UserWarn($"清理了 {offlines.Count} 条");
                        });
                    }
                });
            }, this.GetType());
        }

        private List<string> GetOfflineAddress(Dictionary<string, DateTime> dic) {
            var offlines = new List<string>();
            // 如果WsServer节点服务器的时间和WebApiServer的时间相差很多则该WsServer会被下线
            var dt = DateTime.Now.AddSeconds(-30);
            foreach (var item in dic) {
                if (item.Value <= dt) {
                    offlines.Add(item.Key);
                }
            }
            return offlines;
        }
    }
}
