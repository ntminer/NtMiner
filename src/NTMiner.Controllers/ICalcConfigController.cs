using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface ICalcConfigController {
        DataResponse<List<CalcConfigData>> CalcConfigs(CalcConfigsRequest request);
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
    }
}
