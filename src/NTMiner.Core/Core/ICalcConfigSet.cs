using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICalcConfigSet : IEnumerable<CalcConfigData> {
        bool TryGetCalcConfig(ICoin coin, out ICalcConfig calcConfig);
        IncomePerDay GetIncomePerHashPerDay(ICoin coin);
        void SaveCalcConfigs(List<CalcConfigData> data);
    }
}
