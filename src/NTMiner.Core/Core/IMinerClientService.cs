using System;
using System.ServiceModel;

namespace NTMiner.Core {
    [ServiceContract]
    public interface IMinerClientService : IDisposable {
        [OperationContract]
        bool ShowMainWindow();

        [OperationContract]
        void StartMine(Guid workId, DateTime timestamp);

        [OperationContract]
        void StopMine(DateTime timestamp);

        [OperationContract]
        void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp);
    }
}
