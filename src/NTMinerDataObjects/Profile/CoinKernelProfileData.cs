using LiteDB;
using System;
using System.Text;

namespace NTMiner.Profile {
    public class CoinKernelProfileData : ICoinKernelProfile, IDbEntity<Guid>, IGetSignData {
        public CoinKernelProfileData() { }

        public CoinKernelProfileData(ICoinKernelProfile data) {
            this.CoinKernelId = data.CoinKernelId;
            this.IsDualCoinEnabled = data.IsDualCoinEnabled;
            this.IsAutoDualWeight = data.IsAutoDualWeight;
            this.DualCoinId = data.DualCoinId;
            this.DualCoinWeight = data.DualCoinWeight;
            this.CustomArgs = data.CustomArgs;
        }

        public static CoinKernelProfileData CreateDefaultData(Guid coinKernelId) {
            return new CoinKernelProfileData() {
                CoinKernelId = coinKernelId,
                IsDualCoinEnabled = false,
                IsAutoDualWeight = true,
                DualCoinId = Guid.Empty,
                DualCoinWeight = 30,
                CustomArgs = string.Empty
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

        public override string ToString() {
            return this.BuildSign().ToString();
        }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
