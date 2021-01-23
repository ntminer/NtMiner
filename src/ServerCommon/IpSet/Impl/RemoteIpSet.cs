using System.Collections.Generic;
using System.Net;

namespace NTMiner.IpSet.Impl {
    public class RemoteIpSet : IRemoteIpSet {
        private readonly Dictionary<IPAddress, RemoteIpEntry> _dicByIp = new Dictionary<IPAddress, RemoteIpEntry>();

        public RemoteIpSet() {
            VirtualRoot.BuildEventPath<WsTcpClientAcceptedEvent>("收集Ws客户端IP和端口", LogEnum.None, path: message => {
                if (_dicByIp.TryGetValue(message.RemoteIp, out RemoteIpEntry entry)) {
                    entry.IncActionTimes();
                }
                else {
                    entry = new RemoteIpEntry(message.RemoteIp);
                    entry.IncActionTimes();
                    _dicByIp.Add(message.RemoteIp, entry);
                }
            }, this.GetType());
            VirtualRoot.BuildEventPath<WebApiRequestEvent>("收集WebApi客户端IP和端口", LogEnum.None, path: message => {
                if (_dicByIp.TryGetValue(message.RemoteIp, out RemoteIpEntry entry)) {
                    entry.IncActionTimes();
                }
                else {
                    entry = new RemoteIpEntry(message.RemoteIp);
                    entry.IncActionTimes();
                    _dicByIp.Add(message.RemoteIp, entry);
                }
            }, this.GetType());

            VirtualRoot.BuildEventPath<Per10SecondEvent>("周期找出恶意IP封掉", LogEnum.None, path: message => {
                // TODO:阿里云AuthorizeSecurityGroup
            }, this.GetType());
        }
    }
}
