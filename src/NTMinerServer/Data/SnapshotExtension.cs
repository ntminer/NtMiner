using NTMiner.MinerClient;
using NTMiner.MinerServer;

namespace NTMiner.Data {
    public static class SnapshotExtension {
        public static void Snapshot(this IClientCoinSnapshotSet set, IClientData clientData, SpeedData speedData) {
            ClientCoinSnapshotData dualCoinSnapshotData;
            ClientCoinSnapshotData mainCoinSnapshotData = ClientCoinSnapshotData.Create(clientData, speedData, out dualCoinSnapshotData);
            if (mainCoinSnapshotData != null) {
                set.Add(mainCoinSnapshotData);
            }
            if (dualCoinSnapshotData != null) {
                set.Add(dualCoinSnapshotData);
            }
        }
    }
}
