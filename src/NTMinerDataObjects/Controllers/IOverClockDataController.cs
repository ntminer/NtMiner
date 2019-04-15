using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IOverClockDataController {
        ResponseBase AddOrUpdateOverClockData(DataRequest<OverClockData> request);
        ResponseBase RemoveOverClockData(DataRequest<Guid> request);
        DataResponse<List<OverClockData>> OverClockDatas(OverClockDatasRequest request);
    }
}
