using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();

            private AppSettingServiceFace() { }

            private IAppSettingService CreateService() {
                return ChannelFactory.CreateChannel<IAppSettingService>(MinerServerHost, MinerServerPort);
            }

            #region GetAllAppSettingsAsync
            public void GetAllAppSettingsAsync(Action<GetAppSettingsResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            var response = client.GetAllAppSettings(Guid.NewGuid());
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(null);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            var response = client.GetAppSetting(Guid.NewGuid(), key);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(null);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetAppSettingsAsync
            public void GetAppSettingsAsync(string[] keys, Action<GetAppSettingsResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            var response = client.GetAppSettings(Guid.NewGuid(), keys);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(null);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        SetAppSettingRequest request = new SetAppSettingRequest() {
                            MessageId = messageId,
                            Data = entity,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetAppSetting(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion
        }
    }
}
