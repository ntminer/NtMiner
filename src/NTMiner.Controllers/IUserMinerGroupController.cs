using NTMiner.Core;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IUserMinerGroupController {
        DataResponse<List<UserMinerGroupData>> MinerGroups(SignRequest request);
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
    }
}
