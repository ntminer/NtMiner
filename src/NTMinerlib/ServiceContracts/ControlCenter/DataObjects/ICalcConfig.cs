using System;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    public interface ICalcConfig {
        string CoinCode { get; }
        double Speed { get; }
        string SpeedUnit { get; }
        double IncomePerDay { get; }
        double IncomeUsdPerDay { get; }
        double IncomeCnyPerDay { get; }
        DateTime CreatedOn { get; }
        DateTime ModifiedOn { get; }
    }
}
