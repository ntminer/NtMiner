
namespace NTMiner.Logging {
    using System;

    public interface ILoggingService {
        void Debug(object message);
        void DebugFormatted(string format, params object[] args);
        void InfoDebugLine(object message);
        void InfoDebugLineFormatted(string format, params object[] args);
        void OkDebugLine(object message);
        void WarnDebugLine(object message);
        void WarnDebugLine(object message, Exception exception);
        void WarnDebugLineFormatted(string format, params object[] args);
        void ErrorDebugLine(object message);
        void ErrorDebugLine(object message, Exception exception);
        void ErrorDebugLineFormatted(string format, params object[] args);
        void FatalDebugLine(object message);
        void FatalDebugLine(object message, Exception exception);
        void FatalDebugLineFormatted(string format, params object[] args);


        void InfoWriteLine(object message);
        void InfoWriteLineFormatted(string format, params object[] args);
        void OkWriteLine(object message);
        void WarnWriteLine(object message);
        void WarnWriteLine(object message, Exception exception);
        void WarnWriteLineFormatted(string format, params object[] args);
        void ErrorWriteLine(object message);
        void ErrorWriteLine(object message, Exception exception);
        void ErrorWriteLineFormatted(string format, params object[] args);
        void FatalWriteLine(object message);
        void FatalWriteLine(object message, Exception exception);
        void FatalWriteLineFormatted(string format, params object[] args);

        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
    }
}
