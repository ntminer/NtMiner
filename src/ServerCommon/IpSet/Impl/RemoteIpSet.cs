using System.Collections.Concurrent;
using System.Net;

namespace NTMiner.IpSet.Impl {
    public class RemoteIpSet : IRemoteIpSet {
        private readonly ConcurrentDictionary<IPAddress, RemoteIpEntry> _dicByIp = new ConcurrentDictionary<IPAddress, RemoteIpEntry>();

        public RemoteIpSet() {
            void IncActionTimes(IPAddress remoteIp) {
                if (_dicByIp.TryGetValue(remoteIp, out RemoteIpEntry entry)) {
                    entry.IncActionTimes();
                }
                else {
                    entry = new RemoteIpEntry(remoteIp);
                    entry.IncActionTimes();
                    if (!_dicByIp.TryAdd(remoteIp, entry)) {
                        _dicByIp[remoteIp].IncActionTimes();
                    }
                }
            }
            VirtualRoot.BuildEventPath<WsTcpClientAcceptedEvent>("收集Ws客户端IP和端口", LogEnum.None, path: message => {
                IncActionTimes(message.RemoteIp);
            }, this.GetType());
            VirtualRoot.BuildEventPath<WebApiRequestEvent>("收集WebApi客户端IP和端口", LogEnum.None, path: message => {
                IncActionTimes(message.RemoteIp);
            }, this.GetType());

            VirtualRoot.BuildEventPath<Per10SecondEvent>("周期找出恶意IP封掉", LogEnum.None, path: message => {
                // TODO:阿里云AuthorizeSecurityGroup
            }, this.GetType());
        }
    }
}
