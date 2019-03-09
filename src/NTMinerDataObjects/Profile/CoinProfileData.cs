using LiteDB;
using System;

namespace NTMiner.Profile {
    public class CoinProfileData : ICoinProfile, IDbEntity<Guid> {
        public CoinProfileData() {
            this.IsOverClockGpuAll = true;
        }

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
                IsOverClockEnabled = false,
                IsOverClockGpuAll = true
            };
        }

        public CoinProfileData(ICoinProfile data) {
            this.PoolId = data.PoolId;
            this.Wallet = data.Wallet;
            this.CoinId = data.CoinId;
            this.CoinKernelId = data.CoinKernelId;
            this.IsHideWallet = data.IsHideWallet;
            this.DualCoinPoolId = data.DualCoinPoolId;
            this.DualCoinWallet = data.DualCoinWallet;
            this.IsDualCoinHideWallet = data.IsDualCoinHideWallet;
            this.IsOverClockEnabled = data.IsOverClockEnabled;
            this.IsOverClockGpuAll = data.IsOverClockGpuAll;
        }

        public Guid GetId() {
            return this.CoinId;
        }

        [BsonId]
        public Guid CoinId { get; set; }

        public Guid PoolId { get; set; }
        public string Wallet { get; set; }
        public bool IsHideWallet { get; set; }
        public Guid CoinKernelId { get; set; }
        public Guid DualCoinPoolId { get; set; }
        public string DualCoinWallet { get; set; }
        public bool IsDualCoinHideWallet { get; set; }

        public bool IsOverClockEnabled { get; set; }

        public bool IsOverClockGpuAll { get; set; }

        public override string ToString() {
            return $"{CoinId}{PoolId}{Wallet}{IsHideWallet}{CoinKernelId}{DualCoinPoolId}{DualCoinWallet}{IsDualCoinHideWallet}{IsOverClockEnabled}{IsOverClockGpuAll}";
        }
    }
}
