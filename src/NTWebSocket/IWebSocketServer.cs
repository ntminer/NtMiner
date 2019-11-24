using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace NTWebSocket {
    public interface IWebSocketServer : IDisposable {
        bool IsSecure { get; }
        SslProtocols EnabledSslProtocols { get; }
        IEnumerable<IWebSocketConnection> Conns { get; }
        int ConnCount { get; }
        void Start();
        Action<IWebSocketConnection> OnOpen { get; set; }
        Action<IWebSocketConnection> OnClose { get; set; }
        Action<IWebSocketConnection, string> OnMessage { get; set; }
        Action<IWebSocketConnection, byte[]> OnBinary { get; set; }
        Action<IWebSocketConnection, byte[]> OnPing { get; set; }
        Action<IWebSocketConnection, byte[]> OnPong { get; set; }
        Action<IWebSocketConnection, Exception> OnError { get; set; }
    }
}
