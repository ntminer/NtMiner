using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICalcConfigSet : IReadOnlyCalcConfigSet {
        void SaveCalcConfigs(List<CalcConfigData> data);
    }
}
