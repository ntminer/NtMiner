using LiteDB;
using System;

namespace NTMiner.Core.Profile {
    public class PoolProfileData : IPoolProfile, IDbEntity<Guid> {
        public PoolProfileData() { }

        public static PoolProfileData CreateDefaultData(IPool pool) {
            return new PoolProfileData() {
                PoolId = pool.GetId(),
                UserName = pool.UserName,
                Password = pool.Password
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
