using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Services.Official {
    public class NTMinerWalletService {
        private readonly string _controllerName = RpcRoot.GetControllerName<INTMinerWalletController>();

        public NTMinerWalletService() {
        }

        #region GetNTMinerWalletsAsync
        public void GetNTMinerWalletsAsync(Action<DataResponse<List<NTMinerWalletData>>, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(INTMinerWalletController.NTMinerWallets), null, callback);
        }
        #endregion

        #region AddOrUpdateNTMinerWalletAsync
        public void AddOrUpdateNTMinerWalletAsync(NTMinerWalletData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<NTMinerWalletData> request = new DataRequest<NTMinerWalletData>() {
                Data = entity
            };
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(INTMinerWalletController.AddOrUpdateNTMinerWallet), data: request, callback);
        }
        #endregion

        #region RemoveNTMinerWalletAsync
        public void RemoveNTMinerWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(INTMinerWalletController.RemoveNTMinerWallet), data: request, callback);
        }
        #endregion
    }
}
