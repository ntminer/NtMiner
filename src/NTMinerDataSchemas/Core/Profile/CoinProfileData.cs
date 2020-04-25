using LiteDB;
using System;

namespace NTMiner.Core.Profile {
    public class CoinProfileData : ICoinProfile, IDbEntity<Guid> {
        public CoinProfileData() {
        }

        public static CoinProfileData CreateDefaultData(Guid coinId, Guid poolId, string wallet, Guid coinKernelId) {
            return new CoinProfileData() {
                PoolId = poolId,
                PoolId1 = default,
                Wallet = wallet,
                CoinId = coinId,
                CoinKernelId = coinKernelId,
                IsHideWallet = default,
                DualCoinPoolId = default,
                DualCoinWallet = string.Empty,
                IsDualCoinHideWallet = default,
                CalcInput = 1
            };
        }

        public Guid GetId() {
            return this.CoinId;
        }

        [BsonId]
        public Guid CoinId { get; set; }

        public Guid PoolId { get; set; }
        public Guid PoolId1 { get; set; }
        public string Wallet { get; set; }
        public bool IsHideWallet { get; set; }
        public Guid CoinKernelId { get; set; }
        public Guid DualCoinPoolId { get; set; }
        public string DualCoinWallet { get; set; }
        public bool IsDualCoinHideWallet { get; set; }

        public double CalcInput { get; set; }
    }
}
