using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IPoolController {
        DataResponse<List<PoolData>> Pools(SignRequest request);
        ResponseBase AddOrUpdatePool(DataRequest<PoolData> request);
        ResponseBase RemovePool(DataRequest<Guid> request);
    }
}
