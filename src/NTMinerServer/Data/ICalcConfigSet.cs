using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface ICalcConfigSet {
        List<CalcConfigData> GetAll();
        void SaveCalcConfigs(List<CalcConfigData> data);
    }
}
