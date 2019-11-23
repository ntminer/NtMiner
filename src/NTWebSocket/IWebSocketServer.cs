using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace NTWebSocket {
    public interface IWebSocketServer : IDisposable {
        bool IsSecure { get; }
        SslProtocols EnabledSslProtocols { get; }
        IEnumerable<IWebSocketConnection> Conns { get; }
        int ConnCount { get; }
        void Start(Action<IWebSocketConnection> config);
    }
}
