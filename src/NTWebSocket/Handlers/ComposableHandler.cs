using System;
using System.Collections.Generic;

namespace NTWebSocket.Handlers {
    public class ComposableHandler : IHandler {
        public readonly Func<string, byte[]> Handshake = s => new byte[0];
        public readonly Func<string, byte[]> TextFrame = x => new byte[0];
        public readonly Func<byte[], byte[]> BinaryFrame = x => new byte[0];
        public readonly Action<List<byte>> ReceiveData = delegate { };
        public readonly Func<byte[], byte[]> PingFrame = i => new byte[0];
        public readonly Func<byte[], byte[]> PongFrame = i => new byte[0];
        public readonly Func<int, byte[]> CloseFrame = i => new byte[0];

        private readonly List<byte> _data = new List<byte>();

        public ComposableHandler(
            Func<string, byte[]> handshake = null, 
            Func<string, byte[]> textFrame = null,
            Func<byte[], byte[]> binaryFrame = null,
            Action<List<byte>> receiveData = null,
            Func<byte[], byte[]> pingFrame = null,
            Func<byte[], byte[]> pongFrame = null,
            Func<int, byte[]> closeFrame = null) {
            if (handshake != null) {
                Handshake = handshake;
            }
            if (textFrame != null) {
                TextFrame = textFrame;
            }
            if (binaryFrame != null) {
                BinaryFrame = binaryFrame;
            }
            if (receiveData != null) {
                ReceiveData = receiveData;
            }
            if (pingFrame != null) {
                PingFrame = pingFrame;
            }
            if (pongFrame != null) {
                PongFrame = pongFrame;
            }
            if (closeFrame != null) {
                CloseFrame = closeFrame;
            }
        }

        public byte[] CreateHandshake(string subProtocol = null) {
            return Handshake(subProtocol);
        }

        public void Receive(IEnumerable<byte> data) {
            _data.AddRange(data);

            ReceiveData(_data);
        }

        public byte[] FrameText(string text) {
            return TextFrame(text);
        }

        public byte[] FrameBinary(byte[] bytes) {
            return BinaryFrame(bytes);
        }

        public byte[] FramePing(byte[] bytes) {
            return PingFrame(bytes);
        }

        public byte[] FramePong(byte[] bytes) {
            return PongFrame(bytes);
        }

        public byte[] FrameClose(int code) {
            return CloseFrame(code);
        }
    }
}

