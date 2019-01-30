using System;

namespace NTMiner.Core.Impl {
    internal class CoinShare : ICoinShare {
        public CoinShare() { }

        public Guid CoinId { get; set; }

        public int TotalShareCount {
            get { return AcceptShareCount + RejectCount; }
        }

        public int AcceptShareCount { get; set; }

        public double RejectPercent { get; set; }

        public int RejectCount { get; set; }

        public DateTime ShareOn { get; set; }
    }
}
