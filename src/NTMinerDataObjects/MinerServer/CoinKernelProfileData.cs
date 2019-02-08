using LiteDB;
using System;

namespace NTMiner.MinerServer {
    public class CoinKernelProfileData : ICoinKernelProfile, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public CoinKernelProfileData() { }

        public static CoinKernelProfileData CreateDefaultData(Guid coinKernelId) {
            return new CoinKernelProfileData() {
                CoinKernelId = coinKernelId,
                IsDualCoinEnabled = false,
                IsAutoDualWeight = true,
                DualCoinId = Guid.Empty,
                DualCoinWeight = 30,
                CustomArgs = string.Empty,
                CreatedOn = DateTime.Now,
                ModifiedOn = Timestamp.UnixBaseTime
            };
        }

        public Guid GetId() {
            return this.CoinKernelId;
        }

        [BsonId]
        public Guid CoinKernelId { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public Guid DualCoinId { get; set; }

        public double DualCoinWeight { get; set; }

        public bool IsAutoDualWeight { get; set; }

        public string CustomArgs { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
