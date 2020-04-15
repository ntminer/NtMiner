using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface INTMinerWalletController {
        // 该方法不需要验证，因为挖矿端调用
        DataResponse<List<NTMinerWalletData>> NTMinerWallets();
#if NoDevFee
#else
        ResponseBase AddOrUpdateNTMinerWallet(DataRequest<NTMinerWalletData> request);
        ResponseBase RemoveNTMinerWallet(DataRequest<Guid> request);
#endif
    }
}
