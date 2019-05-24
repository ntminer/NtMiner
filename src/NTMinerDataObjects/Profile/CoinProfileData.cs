using LiteDB;
using System;
using System.Text;

namespace NTMiner.Profile {
    public class CoinProfileData : ICoinProfile, IDbEntity<Guid>, IGetSignData {
        public CoinProfileData() {
        }

        public static CoinProfileData CreateDefaultData(Guid coinId, Guid poolId, string wallet, Guid coinKernelId) {
            return new CoinProfileData() {
                PoolId = poolId,
                PoolId1 = Guid.Empty,
                Wallet = wallet,
                CoinId = coinId,
                CoinKernelId = coinKernelId,
                IsHideWallet = false,
                DualCoinPoolId = Guid.Empty,
                DualCoinWallet = string.Empty,
                IsDualCoinHideWallet = false
            };
        }

        public CoinProfileData(ICoinProfile data) {
            this.PoolId = data.PoolId;
            this.PoolId1 = data.PoolId1;
            this.Wallet = data.Wallet;
            this.CoinId = data.CoinId;
            this.CoinKernelId = data.CoinKernelId;
            this.IsHideWallet = data.IsHideWallet;
            this.DualCoinPoolId = data.DualCoinPoolId;
            this.DualCoinWallet = data.DualCoinWallet;
            this.IsDualCoinHideWallet = data.IsDualCoinHideWallet;
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

        public override string ToString() {
            return $"{CoinId}{PoolId}{PoolId1}{Wallet}{IsHideWallet}{CoinKernelId}{DualCoinPoolId}{DualCoinWallet}{IsDualCoinHideWallet}";
        }

        public StringBuilder GetSignData() {
            return new StringBuilder(this.ToString());
        }
    }
}
