using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.MinerServer;
using NTMiner.User;
using NTMiner.Ws;
using System;

namespace NTMiner {
    public static class WsCommonService {
        public static string GetUserPassword(WsUserName wsUserName, UserData userData) {
            string password;
            switch (wsUserName.ClientType) {
                case NTMinerAppType.MinerClient:
                    password = string.Empty;
                    break;
                case NTMinerAppType.MinerStudio:
                    password = userData.Password;
                    break;
                default:
                    password = string.Empty;
                    break;
            }
            return password;
        }

        public static void ActiveMinerStudioSession(string sessionId) {
            WsRoot.MinerStudioSessionSet.ActiveByWsSessionId(sessionId, out _);
        }

        public static void ActiveMinerClientSession(string sessionId) {
            if (WsRoot.MinerClientSessionSet.ActiveByWsSessionId(sessionId, out IMinerClientSession minerSession)) {
                WsRoot.MinerClientMqSender.SendMinerClientWsBreathed(minerSession.LoginName, minerSession.ClientId);
            }
        }

        public static void AddMinerStudioSession(IWsSessionAdapter session) {
            if (!WsRoot.TryGetUser(session.AuthorizationBase64, out WsUserName wsUserName, out UserData userData, out string errMsg)) {
                session.CloseAsync(WsCloseCode.Normal, errMsg);
                return;
            }
            IMinerStudioSession minerSession = MinerStudioSession.Create(userData, wsUserName, session.SessionId, WsRoot.WsServer.MinerStudioWsSessionsAdapter);
            WsRoot.MinerStudioSessionSet.Add(minerSession);
            session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ServerTime) {
                Data = Timestamp.GetTimestamp()
            }.SignToBytes(userData.Password));
        }

        public static void AddMinerClientSession(IWsSessionAdapter session) {
            if (!WsRoot.TryGetUser(session.AuthorizationBase64, out WsUserName wsUserName, out UserData userData, out string errMsg)) {
                session.CloseAsync(WsCloseCode.Normal, errMsg);
                return;
            }
            IMinerClientSession minerSession = MinerClientSession.Create(userData, wsUserName, session.SessionId, WsRoot.WsServer.MinerClientWsSessionsAdapter);
            WsRoot.MinerClientSessionSet.Add(minerSession);
            // 通知WebApiServer节点该矿机Ws连线了
            WsRoot.MinerClientMqSender.SendMinerClientWsOpened(minerSession.LoginName, minerSession.ClientId);
            bool isMinerSignChanged;
            if (!WsRoot.MinerSignSet.TryGetByClientId(wsUserName.ClientId, out MinerSign minerSign)) {
                // 此时该矿机是第一次在服务端出现
                minerSign = new MinerSign {
                    Id = LiteDB.ObjectId.NewObjectId().ToString(),
                    ClientId = wsUserName.ClientId,
                    LoginName = userData.LoginName,
                    OuterUserId = wsUserName.UserId,
                    AESPassword = string.Empty,
                    AESPasswordOn = Timestamp.UnixBaseTime
                };
                isMinerSignChanged = true;
            }
            else {
                isMinerSignChanged = minerSign.OuterUserId != wsUserName.UserId || minerSign.LoginName != userData.LoginName;
                if (isMinerSignChanged) {
                    minerSign.OuterUserId = wsUserName.UserId;
                    minerSign.LoginName = userData.LoginName;
                }
            }
            // 通常执行不到，因为用户注册的时候已经生成了RSA公私钥对了
            if (string.IsNullOrEmpty(userData.PublicKey) || string.IsNullOrEmpty(userData.PrivateKey)) {
                var key = Cryptography.RSAUtil.GetRASKey();
                userData.PublicKey = key.PublicKey;
                userData.PrivateKey = key.PrivateKey;
                WsRoot.UserMqSender.SendUpdateUserRSAKey(userData.LoginName, key);
            }
            DateTime now = DateTime.Now;
            if (string.IsNullOrEmpty(minerSign.AESPassword) || minerSign.AESPasswordOn.AddDays(1) < now) {
                isMinerSignChanged = true;
                minerSign.AESPassword = Cryptography.AESUtil.GetRandomPassword();
                minerSign.AESPasswordOn = now;
            }
            session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.UpdateAESPassword) {
                Data = new AESPassword {
                    PublicKey = userData.PublicKey,
                    Password = Cryptography.RSAUtil.EncryptString(minerSign.AESPassword, userData.PrivateKey)
                }
            }.SignToJson(minerSign.AESPassword));
            if (isMinerSignChanged) {
                WsRoot.MinerClientMqSender.SendChangeMinerSign(minerSign);
            }
        }

        public static void RemoveMinerStudioSession(string sessionId) {
            WsRoot.MinerStudioSessionSet.RemoveByWsSessionId(sessionId);
        }

        public static void RemoveMinerClientSession(string sessionId) {
            IMinerClientSession minerSession = WsRoot.MinerClientSessionSet.RemoveByWsSessionId(sessionId);
            if (minerSession != null) {
                WsRoot.MinerClientMqSender.SendMinerClientWsClosed(minerSession.LoginName, minerSession.ClientId);
            }
        }

        public static void HandleMinerStudioMessage(IWsSessionAdapter session, WsMessage message) {
            if (message == null) {
                return;
            }
            if (!WsRoot.MinerStudioSessionSet.TryGetByWsSessionId(session.SessionId, out IMinerStudioSession minerSession)) {
                session.CloseAsync(WsCloseCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            if (!minerSession.IsValid(message)) {
                session.CloseAsync(WsCloseCode.Normal, "意外，签名验证失败，请重新连接");
                return;
            }
            if (message.Type == WsMessage.QueryClientDatas) {
                if (message.TryGetData(out QueryClientsRequest query)) {
                    // 走的内网，因为WsServer启动时会设置内网Rpc地址
                    RpcRoot.OfficialServer.ClientDataBinaryService.QueryClientsForWsAsync(QueryClientsForWsRequest.Create(query, minerSession.LoginName), (QueryClientsResponse response, Exception ex) => {
                        if (response.IsSuccess()) {
                            var userData = WsRoot.ReadOnlyUserSet.GetUser(UserId.CreateLoginNameUserId(minerSession.LoginName));
                            if (userData != null) {
                                session.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.ClientDatas) {
                                    Data = response
                                }.SignToBytes(userData.Password));
                            }
                        }
                    });
                }
                return;
            }
            if (MinerStudioWsMessageHandler.TryGetHandler(message.Type, out Action<string, WsMessage> handler)) {
                try {
                    handler.Invoke(minerSession.LoginName, message);
                }
                catch (Exception ex) {
                    Logger.ErrorDebugLine(ex);
                }
            }
            else {
                NTMinerConsole.UserWarn($"{session.TypeName} {nameof(HandleMinerStudioMessage)} Received InvalidType {message.Type}");
            }
        }

        public static void HandleMinerClientMessage(IWsSessionAdapter session, WsMessage message) {
            if (message == null) {
                return;
            }
            if (!WsRoot.MinerClientSessionSet.TryGetByWsSessionId(session.SessionId, out IMinerClientSession minerSession)) {
                session.CloseAsync(WsCloseCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            else if (MinerClientWsMessageHandler.TryGetHandler(message.Type, out Action<IWsSessionAdapter, string, Guid, WsMessage> handler)) {
                try {
                    handler.Invoke(session, minerSession.LoginName, minerSession.ClientId, message);
                }
                catch (Exception ex) {
                    Logger.ErrorDebugLine(ex);
                }
            }
            else {
                NTMinerConsole.UserWarn($"{session.TypeName} {nameof(HandleMinerClientMessage)} Received InvalidType {message.Type}");
            }
        }

        public static T ParseWsMessage<T>(string json) {
            if (string.IsNullOrEmpty(json) || json[0] != '{' || json[json.Length - 1] != '}') {
                return default;
            }
            return VirtualRoot.JsonSerializer.Deserialize<T>(json);
        }
    }
}
