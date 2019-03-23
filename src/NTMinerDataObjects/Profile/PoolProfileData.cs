using LiteDB;
using System;
using NTMiner.Core;
using System.Text;

namespace NTMiner.Profile {
    public class PoolProfileData : IPoolProfile, IDbEntity<Guid>, IGetSignData {
        public PoolProfileData() { }

        public PoolProfileData(IPoolProfile data) {
            this.PoolId = data.PoolId;
            this.UserName = data.UserName;
            this.Password = data.Password;
        }

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
            return $"{PoolId}{UserName}{Password}";
        }

        public StringBuilder GetSignData() {
            return new StringBuilder(this.ToString());
        }
    }
}
