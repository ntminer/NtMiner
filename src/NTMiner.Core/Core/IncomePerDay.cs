namespace NTMiner.Core {
    public struct IncomePerDay {
        public static readonly IncomePerDay Zero = new IncomePerDay(0, 0, 0);

        public IncomePerDay(double incomeCoin, double incomeUsd, double incomeCny) {
            this.IncomeCoin = incomeCoin;
            this.IncomeUsd = incomeUsd;
            this.IncomeCny = incomeCny;
        }

        public double IncomeCoin { get; private set; }
        public double IncomeUsd { get; private set; }
        public double IncomeCny { get; private set; }
    }
}
