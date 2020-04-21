using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.User;
using NTMiner.Ws;
using System;
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
            if (!this.TryGetUser(out WsUserName wsUserName, out UserData userData)) {
                this.CloseAsync();
                return;
            }
            IMinerStudioSession minerSession = MinerStudioSession.Create(userData, wsUserName, this.ID);
            WsRoot.MinerStudioSessionSet.Add(minerSession);
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
            if (!WsRoot.MinerStudioSessionSet.TryGetByWsSessionId(this.ID, out IMinerStudioSession minerSession)) {
                this.CloseAsync(CloseStatusCode.Normal, "意外，会话不存在，请重新连接");
                return;
            }
            if (!minerSession.IsValid(message)) {
                this.CloseAsync(CloseStatusCode.Normal, "意外，签名验证失败，请重新连接");
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
                Write.UserWarn($"{_behaviorName} {nameof(OnMessage)} Received InvalidType {e.Data}");
            }
        }
    }
}
