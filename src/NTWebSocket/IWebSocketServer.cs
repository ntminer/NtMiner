using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace NTWebSocket {
    public interface IWebSocketServer : IDisposable {
        bool IsSecure { get; }
        SslProtocols EnabledSslProtocols { get; }
        IEnumerable<IWebSocketConnection> Conns { get; }
        int ConnCount { get; }
        void Start(
            Action<IWebSocketConnection> onOpen = null,
            Action<IWebSocketConnection> onClose = null,
            Action<IWebSocketConnection, string> onMessage = null,
            Action<IWebSocketConnection, byte[]> onBinary = null,
            Action<IWebSocketConnection, byte[]> onPing = null,
            Action<IWebSocketConnection, byte[]> onPong = null,
            Action<IWebSocketConnection, Exception> onError = null);

        event Action<IWebSocketConnection> Opened;
        event Action<IWebSocketConnection> Closed;
        event Action<IWebSocketConnection, Exception> Error;
        void OnOpen(IWebSocketConnection conn);
        void OnClose(IWebSocketConnection conn);
        void OnMessage(IWebSocketConnection conn, string message);
        void OnBinary(IWebSocketConnection conn, byte[] data);
        void OnPing(IWebSocketConnection conn, byte[] data);
        void OnPong(IWebSocketConnection conn, byte[] data);
        void OnError(IWebSocketConnection conn, Exception e);
    }
}
