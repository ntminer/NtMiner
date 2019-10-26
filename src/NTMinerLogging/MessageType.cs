using System;

namespace NTMiner {
    public enum MessageType {
        Default,
        Error,
        Warn,
        Ok,
        Fail,
        Info,
        Debug,
        TimeSpan
    }

    public static class MessageTypeExtension {
        public static ConsoleColor ToConsoleColor(this MessageType messageType) {
            switch (messageType) {
                case MessageType.Error:
                    return ConsoleColor.Red;
                case MessageType.Warn:
                    return ConsoleColor.Yellow;
                case MessageType.Ok:
                    return ConsoleColor.Green;
                case MessageType.Fail:
                    return ConsoleColor.Red;
                case MessageType.Info:
                    return ConsoleColor.Gray;
                case MessageType.TimeSpan:
                    return ConsoleColor.Gray;
                case MessageType.Debug:
                case MessageType.Default:
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
