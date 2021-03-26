using System;

namespace NTMiner.Core {
    public class CoinData : ICoin, IDbEntity<Guid> {
        public CoinData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public string EnName { get; set; }

        public string CnName { get; set; }

        public string Icon { get; set; }

        public Guid AlgoId { get; set; }

        public string Algo { get; set; }

        public string TestWallet { get; set; }

        public string WalletRegexPattern { get; set; }

        public bool JustAsDualCoin { get; set; }

        public string Notice { get; set; }
        public string TutorialUrl { get; set; }

        public bool IsHot { get; set; }

        public string KernelBrand { get; set; }
        // 使导出的json向后兼容，待旧版本用户少了可以去除这个属性

        public int SortNumber { get; set; }

        public double MinGpuMemoryGb { get; set; }
    }
}
