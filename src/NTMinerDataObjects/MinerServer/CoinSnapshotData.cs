using LiteDB;
using System;

namespace NTMiner.MinerServer {
    public class CoinSnapshotData {
        public CoinSnapshotData() { }

        [BsonId(autoId: true)]
        public ObjectId Id { get; set; }
        public string CoinCode { get; set; }
        public double Speed { get; set; }
        public int ShareDelta { get; set; }
        public int MainCoinOnlineCount { get; set; }
        public int MainCoinMiningCount { get; set; }
        public int DualCoinOnlineCount { get; set; }
        public int DualCoinMiningCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
