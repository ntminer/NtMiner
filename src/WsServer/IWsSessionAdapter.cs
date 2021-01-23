using System.Net;

namespace NTMiner {
    public interface IWsSessionAdapter {
        string SessionId { get; }
        void CloseAsync(WsCloseCode code, string reason);
        void SendAsync(string data);
        void SendAsync(byte[] data);
    }
}
