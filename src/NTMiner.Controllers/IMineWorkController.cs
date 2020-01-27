using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IMineWorkController {
        DataResponse<List<MineWorkData>> MineWorks(SignRequest request);
        ResponseBase AddOrUpdateMineWork(DataRequest<MineWorkData> request);
        ResponseBase RemoveMineWork(DataRequest<Guid> request);
        ResponseBase ExportMineWork(ExportMineWorkRequest request);
        DataResponse<string> GetLocalJson(DataRequest<Guid> request);
    }
}
