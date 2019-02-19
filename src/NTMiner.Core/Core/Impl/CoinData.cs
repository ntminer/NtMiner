using System;

namespace NTMiner.Core.Impl {
    public class CoinData : ICoin, IDbEntity<Guid> {
        public static readonly ICoin Empty = new CoinData() {
            Id = Guid.Empty,
            Code = string.Empty,
            EnName = string.Empty,
            CnName = string.Empty,
            Algo = string.Empty,
            TestWallet = string.Empty,
            WalletRegexPattern = string.Empty,
            JustAsDualCoin = false
        };

        public CoinData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public string EnName { get; set; }

        public string CnName { get; set; }

        public string Algo { get; set; }

        public string TestWallet { get; set; }

        public string WalletRegexPattern { get; set; }

        public bool JustAsDualCoin { get; set; }
    }
}
