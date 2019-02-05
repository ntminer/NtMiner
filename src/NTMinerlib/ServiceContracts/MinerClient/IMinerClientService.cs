using NTMiner;
using System;
using System.ServiceModel;

namespace NTMiner.ServiceContracts.MinerClient {
    [ServiceContract]
    public interface IMinerClientService : IDisposable {
        [OperationContract]
        bool ShowMainWindow();

        [OperationContract]
        ResponseBase StartMine(StartMineRequest request);

        [OperationContract]
        void StopMine(DateTime timestamp);

        [OperationContract]
        void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp);
    }
}
