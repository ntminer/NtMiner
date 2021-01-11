using NTMiner.Ws;
using WebSocketSharp;

namespace NTMiner.WsSharp {
    public class MinerStudioSharpWsSessionAdapter : SharpWsSessionAdapterBase {
        public MinerStudioSharpWsSessionAdapter() : base(NTMinerAppType.MinerStudio) {
        }

        // 查源码可知基类的OnOpen、OnMessage、OnError、OnClose都是空，调用基类对应方法是为了避免第三方代码将来改动
        protected override void OnOpen() {
            base.OnOpen();
            WsCommonService.AddMinerStudioSession(this);
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            WsCommonService.RemoveMinerStudioSession(base.SessionId);
        }

        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);
            if (e.IsPing) {
                WsCommonService.ActiveMinerStudioSession(base.SessionId);
                return;
            }
            WsMessage wsMessage = e.ToWsMessage<WsMessage>();
            WsCommonService.HandleMinerStudioMessage(this, wsMessage);
        }
    }
}
