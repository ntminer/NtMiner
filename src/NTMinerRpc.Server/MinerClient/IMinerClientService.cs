using System;

namespace NTMiner.MinerClient {
    public interface IMinerClientService : IDisposable {
        bool ShowMainWindow();

        ResponseBase StartMine(StartMineInput request);

        void StopMine(DateTime timestamp);

        void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp);
    }
}
