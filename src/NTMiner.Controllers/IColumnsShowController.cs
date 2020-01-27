using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IColumnsShowController {
        DataResponse<List<ColumnsShowData>> ColumnsShows(SignRequest request);
        ResponseBase AddOrUpdateColumnsShow(DataRequest<ColumnsShowData> request);
        ResponseBase RemoveColumnsShow(DataRequest<Guid> request);
    }
}
