using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICalcConfigSet {
        List<CalcConfigData> GetAll();
        void SaveCalcConfigs(List<CalcConfigData> data);
    }
}
