using System;
using System.Threading.Tasks;

namespace NTWebSocket {
    public interface IWebSocketConnection {
        Action OnOpen { get; set; }
        DateTime OpenedOn { get; }
        Action OnClose { get; set; }
        DateTime ClosedOn { get; }
        Action<string> OnMessage { get; set; }
        DateTime MessageOn { get; }
        Action<byte[]> OnBinary { get; set; }
        DateTime BinaryOn { get; }
        Action<byte[]> OnPing { get; set; }
        DateTime PingOn { get; }
        Action<byte[]> OnPong { get; set; }
        DateTime PongOn { get; }
        Action<Exception> OnError { get; set; }
        Task Send(string message);
        Task Send(byte[] message);
        Task SendPing(byte[] message);
        Task SendPong(byte[] message);
        void Close();
        IWebSocketConnectionInfo ConnectionInfo { get; }
        bool IsAvailable { get; }
    }
}
