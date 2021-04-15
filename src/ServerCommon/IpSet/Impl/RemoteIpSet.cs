using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            VirtualRoot.BuildEventPath<WsTcpClientAcceptedEvent>("收集Ws客户端IP和端口", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                IncActionTimes(message.RemoteIp);
            });
            VirtualRoot.BuildEventPath<WebApiRequestEvent>("收集WebApi客户端IP和端口", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                IncActionTimes(message.RemoteIp);
            });

            VirtualRoot.BuildEventPath<Per10SecondEvent>("周期找出恶意IP封掉", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                // TODO:阿里云AuthorizeSecurityGroup
            });
            VirtualRoot.BuildEventPath<Per100MinuteEvent>("清理长久不活跃的记录", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                List<IPAddress> toRemoves = new List<IPAddress>();
                DateTime time = message.BornOn.AddHours(-1);
                foreach (var remoteIp in _dicByIp.Values.ToArray()) {
                    if (remoteIp.LastActionOn < time) {
                        toRemoves.Add(remoteIp.RemoteIp);
                    }
                }
                foreach (var remoteIp in toRemoves) {
                    _dicByIp.TryRemove(remoteIp, out _);
                }
            });
        }

        public int Count {
            get { return _dicByIp.Count; }
        }

        public RemoteIpEntry[] GetTopNRemoteIpEntries(int n) {
            if (n <= 0) {
                return new RemoteIpEntry[0];
            }
            return _dicByIp.Values.OrderByDescending(a => a.ActionTimes).Take(n).ToArray();
        }
    }
}
