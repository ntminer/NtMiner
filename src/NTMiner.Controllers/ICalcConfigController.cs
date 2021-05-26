using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface ICalcConfigController {
        DataResponse<List<CalcConfigData>> CalcConfigs();
        DataResponse<List<CalcConfigData>> Query(string coinCodes);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SaveCalcConfigs(SaveCalcConfigsRequest request);
    }
}
