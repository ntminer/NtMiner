using System;
using NTMiner.OverClock;
using System.Collections.Generic;

namespace NTMiner {
    public interface IOverClockDataController {
        ResponseBase AddOrUpdateOverClockData(DataRequest<OverClockData> request);
        ResponseBase RemoveOverClockData(DataRequest<Guid> request);
        DataResponse<List<OverClockData>> OverClockDatas(OverClockDatasRequest request);
    }
}
