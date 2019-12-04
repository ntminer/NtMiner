using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IWalletController {
        DataResponse<List<WalletData>> Wallets(SignRequest request);
        ResponseBase AddOrUpdateWallet(DataRequest<WalletData> request);
        ResponseBase RemoveWallet(DataRequest<Guid> request);
    }
}
