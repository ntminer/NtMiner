using System;
using System.ServiceModel;

namespace NTMiner.ServiceContracts {
    [ServiceContract]
    public interface ITimeService : IDisposable {
        [OperationContract]
        DateTime GetTime();

        [OperationContract]
        string GetServerPubKey();
    }
}
