using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IClientCoinSnapshotSet {
        DateTime GetMinTimestamp();
        void Add(ClientCoinSnapshotData data);

        List<ClientCoinSnapshotData> GetClientCoinSnapshots(DateTime leftTime, DateTime rightTime);
    }
}
