using NTMiner.OverClock;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class OverClockDataServiceFace {
            public static readonly OverClockDataServiceFace Instance = new OverClockDataServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IOverClockDataController>();

            private OverClockDataServiceFace() { }

            #region GetOverClockDatas
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns></returns>
            public GetOverClockDatasResponse GetOverClockDatas(Guid messageId) {
                try {
                    OverClockDatasRequest request = new OverClockDatasRequest {
                        MessageId = Guid.NewGuid()
                    };
                    GetOverClockDatasResponse response = Request<GetOverClockDatasResponse>(s_controllerName, nameof(IOverClockDataController.OverClockDatas), request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
            #endregion

            #region AddOrUpdateOverClockDataAsync
            public void AddOrUpdateOverClockDataAsync(OverClockData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    AddOrUpdateOverClockDataRequest request = new AddOrUpdateOverClockDataRequest {
                        LoginName = SingleUser.LoginName,
                        Data = entity
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IOverClockDataController.AddOrUpdateOverClockData), request, callback);
                });
            }
            #endregion

            #region RemoveOverClockDataAsync
            public void RemoveOverClockDataAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RemoveOverClockDataRequest request = new RemoveOverClockDataRequest() {
                        LoginName = SingleUser.LoginName,
                        OverClockDataId = id
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IOverClockDataController.RemoveOverClockData), request, callback);
                });
            }
            #endregion
        }
    }
}
