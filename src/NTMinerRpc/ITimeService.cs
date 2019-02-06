using System;

namespace NTMiner.ServiceContracts {
    public interface ITimeService : IDisposable {
        DateTime GetTime();
    }
}
