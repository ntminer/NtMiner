using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Core.MinerServer;
using NTMiner.User;
using NTMiner.Ws;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner {
    public class MinerClientBehavior : WebSocketBehavior {
        public const string WsServiceHostPath = "/" + nameof(NTMinerAppType.MinerClient);
        private const string _behaviorName = nameof(MinerClientBehavior);

        public MinerClientBehavior() {
            this.EmitOnPing = true;
        }

        protected override void OnOpen() {
            base.OnOpen();
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(base.Context.User.Identity.Name));
            WsUserName wsUserName = VirtualRoot.JsonSerializer.Deserialize<WsUserName>(json);
            if (wsUserName == null) {
                this.CloseAsync();
                return;
            }
            UserData userData = WsRoot.ReadOnlyUserSet.GetUser(UserId.Create(wsUserName.UserId));
            if (userData == null) {
                this.CloseAsync();
                return;
            }
            IMinerClientSession minerClientSession = MinerClientSession.Create(userData, wsUserName, this.ID);
            WsRoot.MinerClientSessionSet.Add(minerClientSession);
            WsRoot.MinerClientMqSender.SendMinerClientWsOpened(minerClientSession.LoginName, minerClientSession.ClientId);
            if (!WsRoot.MinerSignSet.TryGetByClientId(wsUserName.ClientId, out MinerSign minerSign)) {
                minerSign = new MinerSign {
                    Id = LiteDB.ObjectId.NewObjectId().ToString(),
                    ClientId = wsUserName.ClientId,
                    OuterUserId = wsUserName.UserId,
                    AESPassword = string.Empty,
                    AESPasswordOn = Timestamp.UnixBaseTime
                };
            }
            bool isMinerSignChanged = minerSign.OuterUserId != wsUserName.UserId;
            if (isMinerSignChanged) {
                minerSign.Update(wsUserName);
            }
            if (string.IsNullOrEmpty(userData.PublicKey) || string.IsNullOrEmpty(userData.PrivateKey)) {
                var key = Cryptography.RSAHelper.GetRASKey();
                userData.PublicKey = key.PublicKey;
                userData.PrivateKey = key.PrivateKey;
                WsRoot.UserMqSender.SendUpdateUserRSAKey(userData.LoginName, key);
            }
            DateTime now = DateTime.Now;
            if (string.IsNullOrEmpty(minerSign.AESPassword) || minerSign.AESPasswordOn.AddDays(1) < now) {
                isMinerSignChanged = true;
                minerSign.AESPassword = Cryptography.AESHelper.GetRandomPassword();
                minerSign.AESPasswordOn = now;
            }
            WsRoot.MinerClientSessionSet.SendToWsClientAsync(this.ID, new WsMessage(Guid.NewGuid(), WsMessage.UpdateAESPassword) {
                Data = new AESPassword {
                    PublicKey = userData.PublicKey,
                    Password = Cryptography.RSAHelper.EncryptString(minerSign.AESPassword, userData.PrivateKey)
                }
            });
            if (isMinerSignChanged) {
                WsRoot.MinerClientMqSender.SendChangeMinerSign(minerSign);
            }
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            IMinerClientSession minerClientSession = WsRoot.MinerClientSessionSet.RemoveByWsSessionId(base.ID);
            if (minerClientSession != null) {
                WsRoot.MinerClientMqSender.SendMinerClientWsClosed(minerClientSession.LoginName, minerClientSession.ClientId);
            }
        }

        protected override void OnMessage(MessageEventArgs e) {
            if (e.IsPing) {
                if (WsRoot.MinerClientSessionSet.ActiveByWsSessionId(base.ID, out IMinerClientSession ntminerSession)) {
                    WsRoot.MinerClientMqSender.SendMinerClientWsBreathed(ntminerSession.LoginName, ntminerSession.ClientId);
                }
                return;
            }
            WsMessage message = e.ToWsMessage<WsMessage>();
            if (message == null) {
                return;
            }
            if (!WsRoot.MinerClientSessionSet.TryGetByWsSessionId(this.ID, out IMinerClientSession minerClientSession)) {
                this.CloseAsync(CloseStatusCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            else if (MinerClientWsMessageHandler.TryGetHandler(message.Type, out Action<MinerClientBehavior, string, Guid, WsMessage> handler)) {
                try {
                    handler.Invoke(this, minerClientSession.LoginName, minerClientSession.ClientId, message);
                }
                catch (Exception ex) {
                    Logger.ErrorDebugLine(ex);
                }
            }
            else {
                Write.UserWarn($"{_behaviorName} {nameof(OnMessage)} Received InvalidType {e.Data}");
            }
        }
    }
}
