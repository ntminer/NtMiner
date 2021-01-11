using NTMiner.Ws;
using WebSocketSharp;

namespace NTMiner.WsSharp {
    public class MinerClientSharpWsSessionAdapter : SharpWsSessionAdapterBase {
        public MinerClientSharpWsSessionAdapter() : base(NTMinerAppType.MinerClient) {
        }

        // 查源码可知基类的OnOpen、OnMessage、OnError、OnClose都是空，调用基类对应方法是为了避免第三方代码将来改动
        protected override void OnOpen() {
            base.OnOpen();
            WsCommonService.AddMinerClientSession(this);
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            WsCommonService.RemoveMinerClientSession(base.SessionId);
        }

        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);
            if (e.IsPing) {
                WsCommonService.ActiveMinerClientSession(base.SessionId);
                return;
            }
            WsMessage wsMessage = e.ToWsMessage<WsMessage>();
            WsCommonService.HandleMinerClientMessage(this, wsMessage);
        }
    }
}
