using NTMiner.Core.MinerServer;
using NTMiner.User;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner.Core.Impl {
    public abstract class AbstractSessionSet<TSession> : ISessionSet<TSession> where TSession : ISession {
        protected static bool IsValid(Guid clientId, DateTime timestamp, string loginName) {
            if (clientId == Guid.Empty) {
                return false;
            }
            if (IsTooOld(timestamp)) {
                return false;
            }
            IUser user = WsRoot.ReadOnlyUserSet.GetUser(UserId.CreateLoginNameUserId(loginName));
            if (user == null) {
                return false;
            }
            if (!WsRoot.MinerSignSet.TryGetByClientId(clientId, out MinerSign minerSign) || !minerSign.IsOwnerBy(user)) {
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
        private readonly string _wsServiceHostPath;
        private static readonly Type _sessionType = typeof(TSession);
        private static readonly bool _isMinerClient = _sessionType == typeof(IMinerClientSession);
        private static readonly bool _isMinerStudio = _sessionType == typeof(IMinerStudioSession);
        public AbstractSessionSet(string wsServiceHostPath) {
            _wsServiceHostPath = wsServiceHostPath;
            VirtualRoot.AddEventPath<CleanTimeArrivedEvent>("打扫时间到，保持清洁", LogEnum.UserConsole, action: message => {
                ClearDeath();
                SendReGetServerAddressMessage(message.NodeAddresses);
            }, this.GetType());
            VirtualRoot.AddEventPath<UserDisabledMqMessage>("收到了UserDisabledMq消息后断开该用户的连接", LogEnum.UserConsole, action: message => {
                if (!string.IsNullOrEmpty(message.LoginName)) {
                    if (TryGetWsSessions(out WebSocketSessionManager wsSessionManager)) {
                        TSession[] toCloses;
                        lock (_locker) {
                            toCloses = _dicByWsSessionId.Values.Where(a => a.LoginName == message.LoginName).ToArray();
                        }
                        foreach (var item in toCloses) {
                            if (wsSessionManager.TryGetSession(item.WsSessionId, out IWebSocketSession session)) {
                                session.Context.WebSocket.CloseAsync(CloseStatusCode.Normal, "用户已被禁用");
                            }
                        }
                    }
                }
            }, this.GetType());
        }

        private void SendReGetServerAddressMessage(string[] nodeAddresses) {
            if (nodeAddresses == null || nodeAddresses.Length == 0) {
                return;
            }
            var thisNodeIp = ServerRoot.HostConfig.ThisServerAddress;
            ShardingHasher hash = new ShardingHasher(nodeAddresses);
            List<TSession> needReGetServerAddressSessions;
            lock (_locker) {
                needReGetServerAddressSessions = _dicByWsSessionId.Values.Where(a => hash.GetTargetNode(a.ClientId) != thisNodeIp).ToList();
            }
            if (needReGetServerAddressSessions.Count != 0) {
                if (TryGetWsSessions(out WebSocketSessionManager wsSessionManager)) {
                    if (_isMinerClient) {
                        foreach (var session in needReGetServerAddressSessions) {
                            string password = ((IMinerClientSession)session).GetSignPassword();
                            try {
                                wsSessionManager.SendToAsync(new WsMessage(Guid.NewGuid(), WsMessage.ReGetServerAddress).SignToJson(password), session.WsSessionId, completed: null);
                            }
                            catch {
                            }
                        }
                    }
                    else if (_isMinerStudio) {
                        foreach (var session in needReGetServerAddressSessions) {
                            var userData = WsRoot.ReadOnlyUserSet.GetUser(UserId.CreateLoginNameUserId(session.LoginName));
                            if (userData != null) {
                                try {
                                    wsSessionManager.SendToAsync(new WsMessage(Guid.NewGuid(), WsMessage.ReGetServerAddress).SignToJson(userData.Password), session.WsSessionId, completed: null);
                                }
                                catch {
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清理掉在给定的时间内未活跃的客户端
        /// </summary>
        /// <param name="seconds"></param>
        private void ClearDeath() {
            // 客户端每20秒ping一次服务端，所以如果2分钟未活跃可以视为不在线了
            int seconds = 120;
            DateTime activeOn = DateTime.Now.AddSeconds(-seconds);
            DateTime doubleActiveOn = activeOn.AddSeconds(-seconds);
            Dictionary<string, TSession> toRemoves;
            lock (_locker) {
                toRemoves = _dicByWsSessionId.Values.Where(a => a != null && a.ActiveOn <= activeOn).ToDictionary(a => a.WsSessionId, a => a);
            }
            if (TryGetWsSessions(out WebSocketSessionManager wsSessionManager)) {
                List<WebSocket> toCloseWses = new List<WebSocket>();
                foreach (var wsSession in wsSessionManager.Sessions) {
                    if (toRemoves.ContainsKey(wsSession.ID)) {
                        toCloseWses.Add(wsSession.Context.WebSocket);
                    }
                }
                foreach (var ws in toCloseWses) {
                    try {
                        ws.CloseAsync(CloseStatusCode.Normal, $"{seconds.ToString()}秒内未活跃");
                    }
                    catch {
                    }
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
                Write.UserWarn($"周期清理不活跃的{_sessionType.Name}，清理了 {toRemoves.Count.ToString()}/{toRemoves.Count.ToString()} 条");
            }
        }

        public int Count {
            get {
                lock (_locker) {
                    return _dicByClientId.Count;
                }
            }
        }

        private WebSocketSessionManager _wsSessions = null;
        public bool TryGetWsSessions(out WebSocketSessionManager wsSessions) {
            if (_wsSessions == null) {
                WsRoot.TryGetWsSessions(_wsServiceHostPath, out _wsSessions);
            }
            wsSessions = _wsSessions;
            return wsSessions != null;
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
                if (TryGetByClientId(ntminerSession.ClientId, out TSession sessionByClientId)) {
                    if (sessionByClientId.WsSessionId == wsSessionId) {
                        ntminerSession.Active();
                        return true;
                    }
                }
                else {
                    lock (_locker) {
                        _dicByClientId[ntminerSession.ClientId] = ntminerSession;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
