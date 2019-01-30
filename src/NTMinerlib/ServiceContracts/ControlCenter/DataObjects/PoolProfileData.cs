using LiteDB;
using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class PoolProfileData : IPoolProfile, IDbEntity<Guid> {
        public PoolProfileData() { }

        public static PoolProfileData CreateDefaultData(Guid poolId) {
            return new PoolProfileData() {
                PoolId = poolId,
                UserName = string.Empty,
                Password = string.Empty
            };
        }

        public Guid GetId() {
            return this.PoolId;
        }

        [BsonId]
        [DataMember]
        public Guid PoolId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
