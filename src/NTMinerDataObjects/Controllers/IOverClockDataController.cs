using System;
using System.Collections.Generic;
using NTMiner.MinerServer;

namespace NTMiner.Controllers {
    public interface IOverClockDataController {
        ResponseBase AddOrUpdateOverClockData(DataRequest<OverClockData> request);
        ResponseBase RemoveOverClockData(DataRequest<Guid> request);
        DataResponse<List<OverClockData>> OverClockDatas(OverClockDatasRequest request);
    }
}
