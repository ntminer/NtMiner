
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
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
    }
}
