using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class SpeedData : RequestBase {
        public SpeedData() { }

        [DataMember]
        public Guid ClientId { get; set; }

        [DataMember]
        public string MinerName { get; set; }

        [DataMember]
        public string MainCoinCode { get; set; }

        [DataMember]
        public string MainCoinPool { get; set; }

        [DataMember]
        public string MainCoinWallet { get; set; }

        [DataMember]
        public string Kernel { get; set; }

        [DataMember]
        public string DualCoinCode { get; set; }

        [DataMember]
        public string DualCoinPool { get; set; }

        [DataMember]
        public string DualCoinWallet { get; set; }

        [DataMember]
        public int MainCoinShareDelta { get; set; }

        [DataMember]
        public bool IsDualCoinEnabled { get; set; }

        [DataMember]
        public int DualCoinShareDelta { get; set; }

        [DataMember]
        public long MainCoinSpeed { get; set; }

        [DataMember]
        public long DualCoinSpeed { get; set; }

        [DataMember]
        public bool IsMining { get; set; }
    }
}
