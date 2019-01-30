using LiteDB;
using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class CoinSnapshotData {
        public CoinSnapshotData() { }

        [BsonId(autoId: true)]
        public ObjectId Id { get; set; }
        [DataMember]
        public string CoinCode { get; set; }
        [DataMember]
        public long Speed { get; set; }
        [DataMember]
        public int ShareDelta { get; set; }
        [DataMember]
        public int MainCoinOnlineCount { get; set; }
        [DataMember]
        public int MainCoinMiningCount { get; set; }
        [DataMember]
        public int DualCoinOnlineCount { get; set; }
        [DataMember]
        public int DualCoinMiningCount { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}
