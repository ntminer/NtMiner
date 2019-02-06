using System;

namespace NTMiner {
    public interface ITimeService : IDisposable {
        DateTime GetTime();
    }
}
