namespace NTMiner {
    public class IncomeItem {
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
            return $"DataCode={DataCode},CoinCode={CoinCode},Speed={Speed.ToString()},SpeedUnit={SpeedUnit},IncomeCoin={IncomeCoin.ToString()},IncomeUsd={IncomeUsd.ToString()},IncomeCny={IncomeCny.ToString()},NetSpeed={NetSpeed.ToString()},NetSpeedUnit={NetSpeedUnit}";
        }
    }
}
