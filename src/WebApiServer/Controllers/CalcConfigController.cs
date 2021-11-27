using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class CalcConfigController : ApiControllerBase, ICalcConfigController {
        #region CalcConfigs
        // 挖矿端实时展示理论收益的功能需要调用此服务所以调用此方法不需要登录
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coinCodes">逗号分割</param>
        /// <returns></returns>
        [Role.Public]
        [HttpGet]
        [HttpPost]
        public DataResponse<List<CalcConfigData>> CalcConfigs() {
            return DoCalcConfigs(string.Empty);
        }

        [Role.Public]
        [HttpGet]
        [HttpPost]
        public DataResponse<List<CalcConfigData>> Query(string coinCodes) {
            return DoCalcConfigs(coinCodes);
        }
        #endregion

        /// <summary>
        /// 获取给定币种列表的收益计算器数据的目的是为了消减自动获取时的数据尺寸从而消减带宽，
        /// 因为阿里云的带宽收费是阶梯递增的。
        /// </summary>
        /// <param name="coinCodes"></param>
        /// <returns></returns>
        internal static DataResponse<List<CalcConfigData>> DoCalcConfigs(string coinCodes) {
            return DataResponse<List<CalcConfigData>>.Ok(AppRoot.CalcConfigSet.Gets(coinCodes));
        }

        #region SaveCalcConfigs
        [Role.Admin]
        [HttpPost]
        public ResponseBase SaveCalcConfigs([FromBody]SaveCalcConfigsRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                AppRoot.CalcConfigSet.SaveCalcConfigs(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
        #endregion
    }
}
