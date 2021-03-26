namespace NTMiner {
    public class IncomeItem {
        public IncomeItem() { }

        public string DataCode { get; set; }
        public string CoinCode { get; set; }
        public double Speed { get; set; }
        public string SpeedUnit { get; set; }
        public double IncomeCoin { get; set; }
        public double IncomeUsd { get; set; }
        public double IncomeCny { get; set; }
        public double NetSpeed { get; set; }
        public string NetSpeedUnit { get; set; }

        public override string ToString() {
            return VirtualRoot.JsonSerializer.Serialize(this);
        }
    }
}
