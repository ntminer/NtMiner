using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface ICalcConfigSet {
        List<CalcConfigData> GetCalcConfigs();
        void SaveCalcConfigs(List<CalcConfigData> data);
    }
}
