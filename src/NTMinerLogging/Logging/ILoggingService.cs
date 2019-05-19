
namespace NTMiner.Logging {
    using System;

    public interface ILoggingService {
        void Debug(object message);
        void InfoDebugLine(object message);
        void OkDebugLine(object message);
        void WarnDebugLine(object message);
        void ErrorDebugLine(object message);
        void ErrorDebugLine(object message, Exception exception);

        void OkWriteLine(object message);
        void WarnWriteLine(object message);
        void EventWriteLine(object message);
        void ErrorWriteLine(object message);
    }
}
