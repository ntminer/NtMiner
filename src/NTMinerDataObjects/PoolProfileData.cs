using LiteDB;
using System;

namespace NTMiner {
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
        public Guid PoolId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
