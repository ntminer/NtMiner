using NTMiner.Core.Redis;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class WsServerNodeAddressSet : WsServerNodeAddressSetBase, IWsServerNodeAddressSet {
        public WsServerNodeAddressSet(IWsServerNodeRedis wsServerNodeRedis) : base(wsServerNodeRedis) {
            VirtualRoot.BuildEventPath<Per5MinuteEvent>("清理掉离线的WsServer节点", LogEnum.None, path: message => {
                wsServerNodeRedis.GetAllAddress().ContinueWith(t => {
                    var offlines = t.Result.Where(a => IsOffline(a.Value, message.BornOn)).Select(a => a.Key).ToArray();
                    if (offlines != null && offlines.Length != 0) {
                        wsServerNodeRedis.ClearAsync(offlines).ContinueWith(_ => {
                            NTMinerConsole.UserWarn($"清理了 {offlines.Length.ToString()} 条");
                        });
                    }
                });
            }, this.GetType());
        }
    }
}
