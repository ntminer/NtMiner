using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Service.OfficialService {
    public class NTMinerWalletServiceFace {
        private static readonly string SControllerName = RpcRoot.GetControllerName<INTMinerWalletController>();

        private readonly string _host;
        private readonly int _port;

        public NTMinerWalletServiceFace(string host, int port) {
            _host = host;
            _port = port;
        }

        #region GetNTMinerWalletsAsync
        public void GetNTMinerWalletsAsync(Action<DataResponse<List<NTMinerWalletData>>, Exception> callback) {
            try {
                NTMinerWalletsRequest request = new NTMinerWalletsRequest {
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(INTMinerWalletController.NTMinerWallets), request, callback);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                callback?.Invoke(null, e);
            }
        }
        #endregion

        #region AddOrUpdateNTMinerWalletAsync
        public void AddOrUpdateNTMinerWalletAsync(NTMinerWalletData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<NTMinerWalletData> request = new DataRequest<NTMinerWalletData>() {
                Data = entity
            };
            RpcRoot.PostAsync(_host, _port, SControllerName, nameof(INTMinerWalletController.AddOrUpdateNTMinerWallet), request, request, callback);
        }
        #endregion

        #region RemoveNTMinerWalletAsync
        public void RemoveNTMinerWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            RpcRoot.PostAsync(_host, _port, SControllerName, nameof(INTMinerWalletController.RemoveNTMinerWallet), request, request, callback);
        }
        #endregion
    }
}
