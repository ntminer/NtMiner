using NTMiner.Core.MinerServer;
using NTMiner.User;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public abstract class AbstractSessionSet<TSession> : ISessionSet<TSession> where TSession : ISession {
        protected static bool IsOwnerBy(Guid clientId, string loginName, DateTime timestamp) {
            if (clientId == Guid.Empty) {
                return false;
            }
            if (IsTooOld(timestamp)) {
                return false;
            }
            IUser user = AppRoot.UserSet.GetUser(UserId.CreateLoginNameUserId(loginName));
            if (user == null || !user.IsEnabled) {
                return false;
            }
            if (!AppRoot.MinerSignSet.TryGetByClientId(clientId, out MinerSign minerSign) || !minerSign.IsOwnerBy(user)) {
                return false;
            }
            return true;
        }

        protected static bool IsTooOld(DateTime dateTime) {
            return dateTime.AddSeconds(30) < DateTime.Now;
        }

        private readonly object _locker = new object();
        private readonly Dictionary<Guid, TSession> _dicByClientId = new Dictionary<Guid, TSession>();
        private readonly Dictionary<string, TSession> _dicByWsSessionId = new Dictionary<string, TSession>();
        private static readonly Type _sessionType = typeof(TSession);
        private readonly IWsSessionsAdapter _wsSessions;

        public AbstractSessionSet(IWsSessionsAdapter wsSessions) {
            this._wsSessions = wsSessions;
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("周期清理死连接、周期通知需要重新连接的客户端重新连接", LogEnum.UserConsole, this.GetType(), PathPriority.Normal, path: message => {
                ClearDeath();
                SendReGetServerAddressMessage(AppRoot.WsServerNodeAddressSet.AsEnumerable().ToArray());
            });
            VirtualRoot.BuildEventPath<UserDisabledMqEvent>("收到了UserDisabledMq消息后断开该用户的连接", LogEnum.UserConsole, this.GetType(), PathPriority.Normal, path: message => {
                if (!string.IsNullOrEmpty(message.LoginName)) {
                    TSession[] toCloses;
                    lock (_locker) {
                        toCloses = _dicByWsSessionId.Values.Where(a => a.LoginName == message.LoginName).ToArray();
                    }
                    foreach (var item in toCloses) {
                        item.CloseAsync(WsCloseCode.Normal, "用户已被禁用");
                    }
                }
            });
        }

        private void SendReGetServerAddressMessage(string[] nodeAddresses) {
            if (nodeAddresses == null || nodeAddresses.Length == 0) {
                return;
            }
            var thisNodeIp = ServerRoot.HostConfig.ThisServerAddress;
            ShardingHasher hash = new ShardingHasher(nodeAddresses);
            List<TSession> needReConnSessions;
            lock (_locker) {
                needReConnSessions = _dicByWsSessionId.Values.Where(a => hash.GetTargetNode(a.ClientId) != thisNodeIp).ToList();
            }
            if (needReConnSessions.Count != 0) {
                foreach (var session in needReConnSessions) {
                    session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ReGetServerAddress) {
                        Data = hash.GetTargetNode(session.ClientId)
                    });
                }
                NTMinerConsole.DevWarn($"通知了 {needReConnSessions.Count.ToString()} 个Ws客户端重新连接");
            }
        }

        /// <summary>
        /// 清理掉在给定的时间内未活跃的客户端
        /// </summary>
        /// <param name="seconds"></param>
        private void ClearDeath() {
            // 客户端每20秒ping一次服务端，所以如果60秒未活跃可以视为不在线了
            int seconds = 60;
            DateTime activeOn = DateTime.Now.AddSeconds(-seconds);
            DateTime doubleActiveOn = activeOn.AddSeconds(-seconds);
            Dictionary<string, TSession> toRemoves;
            lock (_locker) {
                toRemoves = _dicByWsSessionId.Values.Where(a => a != null && a.ActiveOn <= activeOn).ToDictionary(a => a.WsSessionId, a => a);
            }
            foreach (var sessionId in toRemoves.Keys) {
                if (_wsSessions.TryGetSession(sessionId, out IWsSessionAdapter session)) {
                    session.CloseAsync(WsCloseCode.Normal, $"{seconds.ToString()}秒内未活跃");
                }
            }
            // 断开Ws连接时的OnClose事件中会移除已断开连接的会话，但OnClose事件是由WebSocket的库触发
            // 的不是由我触发的，暂时没有深究这个库是否确定会触发OnClose事件所以这里加了个防护逻辑
            foreach (var toRemove in toRemoves) {
                if (toRemove.Value.ActiveOn <= doubleActiveOn) {
                    RemoveByWsSessionId(toRemove.Key);
                }
            }
            if (toRemoves.Count > 0) {
                NTMinerConsole.UserWarn($"周期清理不活跃的{_sessionType.Name}，清理了 {toRemoves.Count.ToString()} 条");
            }
        }

        public int Count {
            get {
                lock (_locker) {
                    return _dicByClientId.Count;
                }
            }
        }

        public virtual void Add(TSession ntminerSession) {
            if (ntminerSession == null) {
                return;
            }
            lock (_locker) {
                if (!_dicByWsSessionId.ContainsKey(ntminerSession.WsSessionId)) {
                    _dicByWsSessionId.Add(ntminerSession.WsSessionId, ntminerSession);
                    _dicByClientId[ntminerSession.ClientId] = ntminerSession;
                }
            }
        }

        public virtual TSession RemoveByWsSessionId(string wsSessionId) {
            lock (_locker) {
                if (_dicByWsSessionId.TryGetValue(wsSessionId, out TSession ntminerSession)) {
                    _dicByWsSessionId.Remove(wsSessionId);
                    if (_dicByClientId.TryGetValue(ntminerSession.ClientId, out ntminerSession) && ntminerSession.WsSessionId == wsSessionId) {
                        _dicByClientId.Remove(ntminerSession.ClientId);
                    }
                }
                return ntminerSession;
            }
        }

        public bool TryGetByClientId(Guid clientId, out TSession ntminerSession) {
            lock (_locker) {
                return _dicByClientId.TryGetValue(clientId, out ntminerSession);
            }
        }

        public bool TryGetByWsSessionId(string wsSessionId, out TSession ntminerSession) {
            lock (_locker) {
                return _dicByWsSessionId.TryGetValue(wsSessionId, out ntminerSession);
            }
        }

        public bool ActiveByWsSessionId(string wsSessionId, out TSession ntminerSession) {
            if (TryGetByWsSessionId(wsSessionId, out ntminerSession)) {
                if (!TryGetByClientId(ntminerSession.ClientId, out TSession _)) {
                    lock (_locker) {
                        _dicByClientId[ntminerSession.ClientId] = ntminerSession;
                    }
                }
                ntminerSession.Active();
                return true;
            }
            return false;
        }
    }
}
