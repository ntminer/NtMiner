using System.Net;

namespace NTMiner {
    public interface IWsSessionAdapter {
        string TypeName { get; }
        string SessionId { get; }
        NTMinerAppType SessionType { get; }
        IPEndPoint RemoteEndPoint { get; }
        string AuthorizationBase64 { get; }
        void CloseAsync(WsCloseCode code, string reason);
        void SendAsync(string data);
        void SendAsync(byte[] data);
    }
}
