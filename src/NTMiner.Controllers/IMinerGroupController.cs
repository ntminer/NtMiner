using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IMinerGroupController {
        DataResponse<List<MinerGroupData>> MinerGroups(SignRequest request);
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
    }
}
