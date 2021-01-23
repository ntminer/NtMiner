using WebSocketSharp.Server;

namespace NTMiner.WsSharp {
    public abstract class SharpWsSessionAdapterBase : WebSocketBehavior, IWsSessionAdapter {
        protected SharpWsSessionAdapterBase() {
            this.EmitOnPing = true;
        }

        public string SessionId {
            get {
                return this.ID;
            }
        }

        public void CloseAsync(WsCloseCode code, string reason) {
            if (base.Context.WebSocket == null
                || base.ConnectionState == WebSocketSharp.WebSocketState.Closing 
                || base.ConnectionState == WebSocketSharp.WebSocketState.Closed) {
                return;
            }
            base.CloseAsync(code.ToCloseStatusCode(), reason);
        }

        public void SendAsync(string data) {
            if (base.ConnectionState != WebSocketSharp.WebSocketState.Open) {
                return;
            }
            base.SendAsync(data, completed: null);
        }

        public void SendAsync(byte[] data) {
            if (base.ConnectionState != WebSocketSharp.WebSocketState.Open) {
                return;
            }
            base.SendAsync(data, completed: null);
        }
    }
}
