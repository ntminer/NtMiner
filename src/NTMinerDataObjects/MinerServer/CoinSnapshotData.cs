using System;
using LiteDB;

namespace NTMiner.MinerServer {
    public class CoinSnapshotData {
        public CoinSnapshotData() {
            Id = ObjectId.NewObjectId().ToString();
        }

        [BsonId]
        public string Id { get; set; }
        public string CoinCode { get; set; }
        public double Speed { get; set; }
        public int ShareDelta { get; set; }
        public int RejectShareDelta { get; set; }
        public int MainCoinOnlineCount { get; set; }
        public int MainCoinMiningCount { get; set; }
        public int DualCoinOnlineCount { get; set; }
        public int DualCoinMiningCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
