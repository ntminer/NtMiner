using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface INTMinerWalletController {
        ResponseBase AddOrUpdateNTMinerWallet(DataRequest<NTMinerWalletData> request);
        ResponseBase RemoveNTMinerWallet(DataRequest<Guid> request);
        DataResponse<List<NTMinerWalletData>> NTMinerWallets(NTMinerWalletsRequest request);
    }
}
