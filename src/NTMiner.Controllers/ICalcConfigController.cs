using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface ICalcConfigController {
        DataResponse<List<CalcConfigData>> CalcConfigs();
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
    }
}
