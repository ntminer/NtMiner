using System;
using System.ServiceModel;

namespace NTMiner.Core {
    [ServiceContract]
    public interface IMinerClientService : IDisposable {
        [OperationContract]
        void StartMine(string desKey, string data);

        [OperationContract]
        void StopMine(string desKey, string data);

        [OperationContract]
        void SetMinerProfileProperty(string desKey, string data);
    }
}
