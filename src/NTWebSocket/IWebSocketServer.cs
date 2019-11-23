using System;
using System.Security.Authentication;

namespace NTWebSocket {
    public interface IWebSocketServer : IDisposable {
        bool IsSecure { get; }
        SslProtocols EnabledSslProtocols { get; }
        void Start(Action<IWebSocketConnection> config);
    }
}
