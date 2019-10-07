using System;

namespace NTWebSocket {
    public interface IWebSocketServer : IDisposable {
        void Start(Action<IWebSocketConnection> config);
    }
}
