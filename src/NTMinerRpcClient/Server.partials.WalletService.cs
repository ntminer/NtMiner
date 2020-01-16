using NTMiner.Controllers;
using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class Server {
        public partial class WalletServiceFace {
            public static readonly WalletServiceFace Instance = new WalletServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IWalletController>();

            private WalletServiceFace() { }

            #region GetWallets
            // TODO:异步化
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public DataResponse<List<WalletData>> GetWallets() {
                try {
                    SignRequest request = new SignRequest {
                    };
                    DataResponse<List<WalletData>> response = Post<DataResponse<List<WalletData>>>(SControllerName, nameof(IWalletController.Wallets), request, request, timeout: 2000);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateWalletAsync
            public void AddOrUpdateWalletAsync(WalletData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<WalletData> request = new DataRequest<WalletData>() {
                    Data = entity
                };
                PostAsync(SControllerName, nameof(IWalletController.AddOrUpdateWallet), request, request, callback);
            }
            #endregion

            #region RemoveWalletAsync
            public void RemoveWalletAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                PostAsync(SControllerName, nameof(IWalletController.RemoveWallet), request, request, callback);
            }
            #endregion
        }
    }
}
