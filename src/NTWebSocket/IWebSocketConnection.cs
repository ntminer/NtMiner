using System;
using System.Threading.Tasks;

namespace NTWebSocket {
    public interface IWebSocketConnection {
        DateTime OpenedOn { get; }
        DateTime MessageOn { get; }
        DateTime BinaryOn { get; }
        DateTime PingOn { get; }
        DateTime PongOn { get; }
        Task Send(string message);
        Task Send(byte[] message);
        Task SendPing(byte[] message);
        Task SendPong(byte[] message);
        void Close();
        IWebSocketConnectionInfo ConnectionInfo { get; }
        bool IsAvailable { get; }
    }
}
