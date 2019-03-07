using NTMiner.MinerClient;

namespace NTMiner.Data {
    public static class SnapshotExtension {
        public static void Snapshot(this IClientCoinSnapshotSet set, SpeedData speedData) {
            ClientCoinSnapshotData dualCoinSnapshotData;
            ClientCoinSnapshotData mainCoinSnapshotData = ClientCoinSnapshotData.Create(speedData, out dualCoinSnapshotData);
            if (mainCoinSnapshotData != null) {
                set.Add(mainCoinSnapshotData);
            }
            if (dualCoinSnapshotData != null) {
                set.Add(dualCoinSnapshotData);
            }
        }
    }
}
