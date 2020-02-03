using System;
using System.IO;
using WebSocketSharp.Server;

namespace NTMiner {
    public class WebSocketBehaviorBase : WebSocketBehavior {
        public new void Error(string message, Exception exception) {
            base.Error(message, exception);
        }
        public new void Send(string data) {
            base.Send(data);
        }
        public new void Send(FileInfo file) {
            base.Send(file);
        }
        public new void Send(byte[] data) {
            base.Send(data);
        }
        public new void SendAsync(FileInfo file, Action<bool> completed) {
            base.SendAsync(file, completed);
        }
        public new void SendAsync(Stream stream, int length, Action<bool> completed) {
            base.SendAsync(stream, length, completed);
        }
        public new void SendAsync(byte[] data, Action<bool> completed) {
            base.SendAsync(data, completed);
        }
        public new void SendAsync(string data, Action<bool> completed) {
            base.SendAsync(data, completed);
        }
    }
}
