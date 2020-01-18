using LiteDB;
using NTMiner.Core;
using System;
using System.Text;

namespace NTMiner.Profile {
    public class PoolProfileData : IPoolProfile, IDbEntity<Guid>, IGetSignData {
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

        public override string ToString() {
            return this.BuildSign().ToString();
        }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
