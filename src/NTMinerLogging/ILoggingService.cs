
namespace NTMiner {
    using System;

    public interface ILoggingService {
        void Debug(object message);
        void InfoDebugLine(object message);
        void OkDebugLine(object message);
        void WarnDebugLine(object message);
        void ErrorDebugLine(object message);
        void ErrorDebugLine(object message, Exception exception);

        void OkUserLine(object message);
        void WarnUserLine(object message);
        void ErrorUserLine(object message);
    }
}
