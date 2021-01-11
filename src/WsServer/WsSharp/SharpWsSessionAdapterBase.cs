using System.Net;
using WebSocketSharp.Server;

namespace NTMiner.WsSharp {
    public abstract class SharpWsSessionAdapterBase : WebSocketBehavior, IWsSessionAdapter {
        protected SharpWsSessionAdapterBase(NTMinerAppType sessionType) {
            this.EmitOnPing = true;
            this.SessionType = sessionType;
        }

        public string TypeName {
            get {
                return this.GetType().Name;
            }
        }

        public NTMinerAppType SessionType { get; private set; }

        public string SessionId {
            get {
                return this.ID;
            }
        }

        public IPEndPoint RemoteEndPoint {
            get {
                return this.Context.UserEndPoint;
            }
        }

        public string AuthorizationBase64 {
            get {
                return this.Context.User.Identity.Name;
            }
        }

        public void CloseAsync(WsCloseCode code, string reason) {
            base.CloseAsync(code.ToCloseStatusCode(), reason);
        }

        public void SendAsync(string data) {
            base.SendAsync(data, completed: null);
        }

        public void SendAsync(byte[] data) {
            base.SendAsync(data, completed: null);
        }
    }
}
