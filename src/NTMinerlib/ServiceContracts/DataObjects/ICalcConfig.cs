using System;

namespace NTMiner.ServiceContracts.DataObjects {
    public interface ICalcConfig {
        string CoinCode { get; }
        double Speed { get; }
        string SpeedUnit { get; }
        double IncomePerDay { get; }
        DateTime CreatedOn { get; }
        DateTime ModifiedOn { get; }
    }
}
