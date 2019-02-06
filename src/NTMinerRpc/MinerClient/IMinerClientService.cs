using System;

namespace NTMiner.ServiceContracts.MinerClient {
    public interface IMinerClientService : IDisposable {
        bool ShowMainWindow();

        ResponseBase StartMine(StartMineRequest request);

        void StopMine(DateTime timestamp);

        void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp);
    }
}
