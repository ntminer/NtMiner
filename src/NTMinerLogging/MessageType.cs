using System;

namespace NTMiner {
    /// <summary>
    /// MessageType这名字听起来像是消息总线上的消息的类型似的，但消息总线上的消息的类型信息是由.NET类的类型承载的所以消息总线那块不会有个叫MessageType的类型。
    /// </summary>
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
