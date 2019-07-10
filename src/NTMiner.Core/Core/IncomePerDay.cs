using System;

namespace NTMiner.Core {
    public struct IncomePerDay {
        public static readonly IncomePerDay Zero = new IncomePerDay(0, 0, 0, DateTime.MinValue);

        public IncomePerDay(double incomeCoin, double incomeUsd, double incomeCny, DateTime modifiedOn) {
            this.IncomeCoin = incomeCoin;
            this.IncomeUsd = incomeUsd;
            this.IncomeCny = incomeCny;
            this.ModifiedOn = modifiedOn;
        }

        public double IncomeCoin { get; private set; }
        public double IncomeUsd { get; private set; }
        public double IncomeCny { get; private set; }
        public DateTime ModifiedOn { get; private set; }
    }
}
