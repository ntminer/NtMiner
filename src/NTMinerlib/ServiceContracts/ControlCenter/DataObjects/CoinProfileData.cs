using LiteDB;
using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class CoinProfileData : ICoinProfile, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public CoinProfileData() { }

        public static CoinProfileData CreateDefaultData(Guid coinId) {
            return new CoinProfileData() {
                PoolId = Guid.Empty,
                Wallet = string.Empty,
                CoinId = coinId,
                CoinKernelId = Guid.Empty,
                IsHideWallet = false,
                DualCoinPoolId = Guid.Empty,
                DualCoinWallet = string.Empty,
                IsDualCoinHideWallet = false,
                CreatedOn = DateTime.Now,
                ModifiedOn = Global.UnixBaseTime
            };
        }

        public Guid GetId() {
            return this.CoinId;
        }

        [BsonId]
        [DataMember]
        public Guid CoinId { get; set; }

        [DataMember]
        public Guid PoolId { get; set; }
        [DataMember]
        public string Wallet { get; set; }
        [DataMember]
        public bool IsHideWallet { get; set; }
        [DataMember]
        public Guid CoinKernelId { get; set; }
        [DataMember]
        public Guid DualCoinPoolId { get; set; }
        [DataMember]
        public string DualCoinWallet { get; set; }
        [DataMember]
        public bool IsDualCoinHideWallet { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
