namespace NTWebSocket {
    public enum FrameType : byte {
        Continuation,
        Text,
        Binary,
        Close = 8,
        Ping = 9,
        Pong = 10,
    }
}

