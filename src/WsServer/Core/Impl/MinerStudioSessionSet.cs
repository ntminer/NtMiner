using NTMiner.User;
using NTMiner.Ws;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MinerStudioSessionSet : AbstractSessionSet<IMinerStudioSession>, IMinerStudioSessionSet {
        private readonly Dictionary<string, List<IMinerStudioSession>> _dicByLoginName = new Dictionary<string, List<IMinerStudioSession>>();

        public MinerStudioSessionSet(IWsSessionsAdapter sessions) : base(sessions) {
            VirtualRoot.BuildEventPath<UserPasswordChangedMqMessage>("群控用户密码变更后通知群控客户端重新登录", LogEnum.None, path: message => {
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.ReLogin));
            }, this.GetType());
            VirtualRoot.BuildEventPath<ConsoleOutLinesMqMessage>("收到ConsoleOutLinesMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.ConsoleOutLines) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<LocalMessagesMqMessage>("收到LocalMessagesMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.LocalMessages) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<DrivesMqMessage>("收到DrivesMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.Drives) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<LocalIpsMqMessage>("收到LocalIpsMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.LocalIps) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<OperationResultsMqMessage>("收到OperationResultsMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.OperationResults) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<OperationReceivedMqMessage>("收到OperationReceivedMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.OperationReceived) {
                    Data = new WrapperClientId {
                        ClientId = message.ClientId
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<LocalJsonMqMessage>("收到LocalJsonMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.SelfWorkLocalJson) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
            VirtualRoot.BuildEventPath<GpuProfilesJsonMqMessage>("收到GpuProfilesJsonMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (!IsValid(message.ClientId, message.Timestamp, message.LoginName)) {
                    return;
                }
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.GpuProfilesJson) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            }, this.GetType());
        }

        public override void Add(IMinerStudioSession minerSession) {
            base.Add(minerSession);
            if (!_dicByLoginName.TryGetValue(minerSession.LoginName, out List<IMinerStudioSession> sessions)) {
                sessions = new List<IMinerStudioSession>();
                _dicByLoginName.Add(minerSession.LoginName, sessions);
            }
            // 一个挖矿端只可能有一个连接，如果建立了新的连接就移除旧的连接
            var toRemoves = sessions.Where(a => a.ClientId == minerSession.ClientId).ToArray();
            foreach (var item in toRemoves) {
                sessions.Remove(item);
            }
            sessions.Add(minerSession);
        }

        public override IMinerStudioSession RemoveByWsSessionId(string wsSessionId) {
            var ntminerSession = base.RemoveByWsSessionId(wsSessionId);
            if (ntminerSession != null) {
                if (_dicByLoginName.TryGetValue(ntminerSession.LoginName, out List<IMinerStudioSession> sessions)) {
                    sessions.Remove(ntminerSession);
                }
            }
            return ntminerSession;
        }

        private List<IMinerStudioSession> GetSessionsByLoginName(string loginName) {
            if (string.IsNullOrEmpty(loginName)) {
                return new List<IMinerStudioSession>();
            }
            if (_dicByLoginName.TryGetValue(loginName, out List<IMinerStudioSession> sessions)) {
                return sessions;
            }
            return new List<IMinerStudioSession>();
        }

        public void SendToMinerStudioAsync(string loginName, WsMessage message) {
            var minerStudioSessions = GetSessionsByLoginName(loginName).ToArray();// 避免发生集合被修改的异常
            foreach (var minerStudioSession in minerStudioSessions) {
                var userData = WsRoot.ReadOnlyUserSet.GetUser(UserId.CreateLoginNameUserId(minerStudioSession.LoginName));
                if (userData != null) {
                    try {
                        minerStudioSession.SendAsync(message, userData.Password);
                    }
                    catch {
                    }
                }
            }
        }
    }
}
