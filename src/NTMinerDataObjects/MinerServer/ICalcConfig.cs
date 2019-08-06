using System;

namespace NTMiner.MinerServer {
    public interface ICalcConfig {
        string CoinCode { get; }
        double Speed { get; }
        string SpeedUnit { get; }
        double NetSpeed { get; }
        string NetSpeedUnit { get; }
        double BaseNetSpeed { get; }
        double DayWave { get; }
        double IncomePerDay { get; }
        double IncomeUsdPerDay { get; }
        double IncomeCnyPerDay { get; }
        DateTime CreatedOn { get; }
        DateTime ModifiedOn { get; }
    }
}
