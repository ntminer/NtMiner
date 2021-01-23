using NTMiner.User;
using NTMiner.Ws;
using WebSocketSharp;

namespace NTMiner.WsSharp {
    public class MinerStudioSharpWsSessionAdapter : SharpWsSessionAdapterBase {
        public MinerStudioSharpWsSessionAdapter() {
        }

        protected override void OnOpen() {
            base.OnOpen();
            if (AppRoot.TryGetUser(this.Context.User.Identity.Name, out WsUserName userName, out UserData userData, out string _)) {
                AppRoot.AddMinerStudioSession(userName, userData, this.Context.UserEndPoint, this);
            }
            else {
                this.CloseAsync(WsCloseCode.Normal, "用户不存在");
            }
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            AppRoot.RemoveMinerStudioSession(base.SessionId);
        }

        protected override void OnError(ErrorEventArgs e) {
            base.OnError(e);
            Logger.ErrorDebugLine(e.Exception);
        }

        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);
            if (e.IsPing) {
                AppRoot.ActiveMinerStudioSession(base.SessionId);
                return;
            }
            WsMessage wsMessage = e.ToWsMessage<WsMessage>();
            AppRoot.HandleMinerStudioMessage(this, wsMessage);
        }
    }
}
