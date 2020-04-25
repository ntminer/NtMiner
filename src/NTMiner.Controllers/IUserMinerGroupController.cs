using NTMiner.Core;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IUserMinerGroupController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<List<UserMinerGroupData>> MinerGroups(object request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase AddOrUpdateMinerGroup(DataRequest<MinerGroupData> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase RemoveMinerGroup(DataRequest<Guid> request);
    }
}
