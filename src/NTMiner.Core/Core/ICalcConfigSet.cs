using NTMiner.ServiceContracts.DataObjects;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICalcConfigSet : IEnumerable<CalcConfigData> {
        bool TryGetCalcConfig(ICoin coin, out ICalcConfig calcConfig);
        double GetIncomePerHashPerDay(ICoin coin);
        void SaveCalcConfigs(List<CalcConfigData> data);
    }
}
