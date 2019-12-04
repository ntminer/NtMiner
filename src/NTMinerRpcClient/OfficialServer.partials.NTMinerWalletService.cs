using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class OfficialServer {
        public class NTMinerWalletServiceFace {
            public static readonly NTMinerWalletServiceFace Instance = new NTMinerWalletServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<INTMinerWalletController>();

            private NTMinerWalletServiceFace() { }

            #region GetNTMinerWalletsAsync
            public void GetNTMinerWalletsAsync(Action<DataResponse<List<NTMinerWalletData>>, Exception> callback) {
                try {
                    NTMinerWalletsRequest request = new NTMinerWalletsRequest {
                    };
                    PostAsync(SControllerName, nameof(INTMinerWalletController.NTMinerWallets), null, request, callback);
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
                PostAsync(SControllerName, nameof(INTMinerWalletController.AddOrUpdateNTMinerWallet), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion

            #region RemoveNTMinerWalletAsync
            public void RemoveNTMinerWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(INTMinerWalletController.RemoveNTMinerWallet), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}
