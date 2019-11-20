using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICalcConfigSet {
        void Init(bool forceRefresh = false);
        bool TryGetCalcConfig(ICoin coin, out ICalcConfig calcConfig);
        IncomePerDay GetIncomePerHashPerDay(string coinCode);
        void SaveCalcConfigs(List<CalcConfigData> data);
        IEnumerable<ICalcConfig> AsEnumerable();
    }
}
