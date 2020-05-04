using System;

namespace NTMiner.User {
    public interface ITryLoginTimes {
        string LoginName { get; }
        int Times { get; }
        DateTime LastTryOn { get; }
    }
}
