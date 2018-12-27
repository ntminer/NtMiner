using LiteDB;
using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class CoinKernelProfileData : ICoinKernelProfile, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public CoinKernelProfileData() { }

        public static CoinKernelProfileData CreateDefaultData(Guid coinKernelId) {
            return new CoinKernelProfileData() {
                CoinKernelId = coinKernelId,
                IsDualCoinEnabled = false,
                DualCoinId = Guid.Empty,
                DualCoinWeight = 30,
                CustomArgs = string.Empty,
                CreatedOn = DateTime.Now,
                ModifiedOn = Global.UnixBaseTime
            };
        }

        public Guid GetId() {
            return this.CoinKernelId;
        }

        [BsonId]
        [DataMember]
        public Guid CoinKernelId { get; set; }

        [DataMember]
        public bool IsDualCoinEnabled { get; set; }

        [DataMember]
        public Guid DualCoinId { get; set; }

        [DataMember]
        public double DualCoinWeight { get; set; }

        [DataMember]
        public string CustomArgs { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }
    }
}
