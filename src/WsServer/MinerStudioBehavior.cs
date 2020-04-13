using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.User;
using NTMiner.Ws;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace NTMiner {
    public class MinerStudioBehavior : WebSocketBehavior {
        public const string WsServiceHostPath = "/" + nameof(NTMinerAppType.MinerStudio);
        private const string _behaviorName = nameof(MinerStudioBehavior);

        public MinerStudioBehavior() {
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
            IMinerStudioSession minerStudioSession = MinerStudioSession.Create(userData, wsUserName, this.ID);
            WsRoot.MinerStudioSessionSet.Add(minerStudioSession);
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            WsRoot.MinerStudioSessionSet.RemoveByWsSessionId(base.ID);
        }

        protected override void OnMessage(MessageEventArgs e) {
            if (e.IsPing) {
                WsRoot.MinerStudioSessionSet.ActiveByWsSessionId(base.ID, out _);
                return;
            }
            WsMessage message = e.ToWsMessage<WsMessage>();
            if (message == null) {
                return;
            }
            if (!WsRoot.MinerStudioSessionSet.TryGetByWsSessionId(this.ID, out IMinerStudioSession minerStudioSession)) {
                this.CloseAsync(CloseStatusCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            if (!minerStudioSession.IsValid(message)) {
                this.CloseAsync(CloseStatusCode.Normal, "意外，签名验证失败，请重新连接");
                return;
            }
            if (MinerStudioWsMessageHandler.TryGetHandler(message.Type, out Action<string, WsMessage> handler)) {
                try {
                    handler.Invoke(minerStudioSession.LoginName, message);
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
