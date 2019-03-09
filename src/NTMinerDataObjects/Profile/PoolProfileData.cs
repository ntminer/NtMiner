using LiteDB;
using System;

namespace NTMiner.Profile {
    public class PoolProfileData : IPoolProfile, IDbEntity<Guid> {
        public PoolProfileData() { }

        public PoolProfileData(IPoolProfile data) {
            this.PoolId = data.PoolId;
            this.UserName = data.UserName;
            this.Password = data.Password;
        }

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

        public override string ToString() {
            return $"{PoolId}{UserName}{Password}";
        }
    }
}
