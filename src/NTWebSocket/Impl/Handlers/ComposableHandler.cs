using System;
using System.Collections.Generic;

namespace NTWebSocket.Impl.Handlers {
    public class ComposableHandler : IHandler {
        private readonly Func<string, byte[]> _handshake = s => new byte[0];
        private readonly Func<string, byte[]> _textFrame = x => new byte[0];
        private readonly Func<byte[], byte[]> _binaryFrame = x => new byte[0];
        private readonly Action<List<byte>> _receiveData = delegate { };
        private readonly Func<byte[], byte[]> _pingFrame = i => new byte[0];
        private readonly Func<byte[], byte[]> _pongFrame = i => new byte[0];
        private readonly Func<int, byte[]> _closeFrame = i => new byte[0];

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
                _handshake = handshake;
            }
            if (textFrame != null) {
                _textFrame = textFrame;
            }
            if (binaryFrame != null) {
                _binaryFrame = binaryFrame;
            }
            if (receiveData != null) {
                _receiveData = receiveData;
            }
            if (pingFrame != null) {
                _pingFrame = pingFrame;
            }
            if (pongFrame != null) {
                _pongFrame = pongFrame;
            }
            if (closeFrame != null) {
                _closeFrame = closeFrame;
            }
        }

        public byte[] CreateHandshake(string subProtocol = null) {
            return _handshake(subProtocol);
        }

        public void Receive(IEnumerable<byte> data) {
            _data.AddRange(data);

            _receiveData(_data);
        }

        public byte[] FrameText(string text) {
            return _textFrame(text);
        }

        public byte[] FrameBinary(byte[] bytes) {
            return _binaryFrame(bytes);
        }

        public byte[] FramePing(byte[] bytes) {
            return _pingFrame(bytes);
        }

        public byte[] FramePong(byte[] bytes) {
            return _pongFrame(bytes);
        }

        public byte[] FrameClose(int code) {
            return _closeFrame(code);
        }
    }
}

