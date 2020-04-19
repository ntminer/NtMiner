using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IUserMineWorkController {
        DataResponse<List<UserMineWorkData>> MineWorks(SignRequest request);
        ResponseBase AddOrUpdateMineWork(DataRequest<MineWorkData> request);
        ResponseBase RemoveMineWork(DataRequest<Guid> request);
        ResponseBase ExportMineWork(ExportMineWorkRequest request);
        DataResponse<string> GetLocalJson(DataRequest<Guid> request);
        GetWorkJsonResponse GetWorkJson(GetWorkJsonRequest request);
    }
}
