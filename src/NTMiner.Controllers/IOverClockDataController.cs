using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IOverClockDataController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateOverClockData(DataRequest<OverClockData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveOverClockData(DataRequest<Guid> request);
        DataResponse<List<OverClockData>> OverClockDatas(object request);
    }
}
