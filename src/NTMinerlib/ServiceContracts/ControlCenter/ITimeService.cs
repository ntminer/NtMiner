using System;
using System.ServiceModel;

namespace NTMiner.ServiceContracts.ControlCenter {
    [ServiceContract]
    public interface ITimeService : IDisposable {
        [OperationContract]
        DateTime GetTime();
    }
}
