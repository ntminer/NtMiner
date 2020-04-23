using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface ICalcConfigController {
        DataResponse<List<CalcConfigData>> CalcConfigs();
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
    }
}
