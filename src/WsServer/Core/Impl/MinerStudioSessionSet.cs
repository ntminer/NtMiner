using NTMiner.User;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class MinerStudioSessionSet : AbstractSessionSet<IMinerStudioSession>, IMinerStudioSessionSet {
        private readonly Dictionary<string, List<IMinerStudioSession>> _dicByLoginName = new Dictionary<string, List<IMinerStudioSession>>();

        public MinerStudioSessionSet(IWsSessionsAdapter wsSessions) : base(wsSessions) {
            VirtualRoot.BuildEventPath<UserPasswordChangedMqEvent>("群控用户密码变更后通知群控客户端重新登录", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.ReLogin));
            });
            VirtualRoot.BuildEventPath<ConsoleOutLinesMqEvent>("收到ConsoleOutLinesMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(ConsoleOutLinesMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.ConsoleOutLines) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<LocalMessagesMqEvent>("收到LocalMessagesMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(LocalMessagesMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.LocalMessages) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<DrivesMqEvent>("收到DrivesMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(DrivesMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.Drives) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<LocalIpsMqEvent>("收到LocalIpsMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(LocalIpsMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.LocalIps) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<OperationResultsMqEvent>("收到OperationResultsMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(OperationResultsMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.OperationResults) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<OperationReceivedMqEvent>("收到OperationReceivedMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(OperationReceivedMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.OperationReceived) {
                    Data = new WrapperClientId {
                        ClientId = message.ClientId
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<LocalJsonMqEvent>("收到LocalJsonMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(LocalJsonMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.SelfWorkLocalJson) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<GpuProfilesJsonMqEvent>("收到GpuProfilesJsonMq消息后检查对应的用户是否登录着本节点，如果是则处理，否则忽略", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfMinerClientTestIdLogElseNothing(message.ClientId, nameof(GpuProfilesJsonMqEvent));
                SendToMinerStudioAsync(message.LoginName, new WsMessage(message.MessageId, WsMessage.GpuProfilesJson) {
                    Data = new WrapperClientIdData {
                        ClientId = message.ClientId,
                        Data = message.Data
                    }
                });
                #endregion
            });
            VirtualRoot.BuildEventPath<QueryClientsForWsResponseMqEvent>("收到QueryClientsResponseMq消息后通过Ws通道发送给群控客户端", LogEnum.None, this.GetType(), PathPriority.Normal, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                ServerRoot.IfStudioClientTestIdLogElseNothing(message.StudioId, nameof(QueryClientsForWsResponseMqEvent));
                var userData = AppRoot.UserSet.GetUser(UserId.CreateLoginNameUserId(message.LoginName));
                if (userData != null && wsSessions.TryGetSession(message.SessionId, out IWsSessionAdapter session)) {
                    session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ClientDatas) {
                        Data = message.Response
                    }.SignToBytes(userData.Password));
                }
                #endregion
            });
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
            // TODO:考虑给每一个登录的MinerStudio用户建立一个会话，由基于LoginName推送改为基于会话Id推送
            var minerStudioSessions = GetSessionsByLoginName(loginName).ToArray();// 避免发生集合被修改的异常
            foreach (var minerStudioSession in minerStudioSessions) {
                ServerRoot.IfStudioClientTestIdLogElseNothing(minerStudioSession.ClientId, $"{nameof(WsMessage)}.{message.Type}");
                minerStudioSession.SendAsync(message);
            }
        }
    }
}
