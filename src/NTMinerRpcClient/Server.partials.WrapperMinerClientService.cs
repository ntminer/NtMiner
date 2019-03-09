using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public class WrapperMinerClientServiceFace {
            public static readonly WrapperMinerClientServiceFace Instance = new WrapperMinerClientServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IWrapperMinerClientController>();

            private WrapperMinerClientServiceFace() {
            }

            public void RestartWindowsAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RestartWindowsRequest innerRequest = new RestartWindowsRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<RestartWindowsRequest> request = new WrapperRequest<RestartWindowsRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.RestartWindows), request, callback);
                });
            }

            public void ShutdownWindowsAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    ShutdownWindowsRequest innerRequest = new ShutdownWindowsRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<ShutdownWindowsRequest> request = new WrapperRequest<ShutdownWindowsRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.ShutdownWindows), request, callback);
                });
            }

            public void OpenNTMinerAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    OpenNTMinerRequest innerRequest = new OpenNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<OpenNTMinerRequest> request = new WrapperRequest<OpenNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.OpenNTMiner), request, callback);
                });
            }

            public void RestartNTMinerAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RestartNTMinerRequest innerRequest = new RestartNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<RestartNTMinerRequest> request = new WrapperRequest<RestartNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.RestartNTMiner), request, callback);
                });
            }

            public void UpgradeNTMinerAsync(IClientData client, string ntminerFileName, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    UpgradeNTMinerRequest innerRequest = new UpgradeNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp,
                        NTMinerFileName = ntminerFileName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<UpgradeNTMinerRequest> request = new WrapperRequest<UpgradeNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.UpgradeNTMiner), request, callback);
                });
            }

            public void CloseNTMinerAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    CloseNTMinerRequest innerRequest = new CloseNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<CloseNTMinerRequest> request = new WrapperRequest<CloseNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.CloseNTMiner), request, callback);
                });
            }

            public void StartMineAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    MinerClient.StartMineRequest innerRequest = new MinerClient.StartMineRequest {
                        ClientIp = client.MinerIp,
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<MinerClient.StartMineRequest> request = new WrapperRequest<MinerClient.StartMineRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.StartMine), request, callback);
                });
            }

            public void StopMineAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    StopMineRequest innerRequest = new StopMineRequest {
                        ClientIp = client.MinerIp,
                        LoginName = SingleUser.LoginName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<StopMineRequest> request = new WrapperRequest<StopMineRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.StopMine), request, callback);
                });
            }

            public void SetClientMinerProfilePropertyAsync(IClientData client, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SetClientMinerProfilePropertyRequest innerRequest = new SetClientMinerProfilePropertyRequest {
                        ClientIp = client.MinerIp,
                        LoginName = SingleUser.LoginName,
                        PropertyName = propertyName,
                        Value = value
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<SetClientMinerProfilePropertyRequest> request = new WrapperRequest<SetClientMinerProfilePropertyRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.SetClientMinerProfileProperty), request, callback);
                });
            }
        }
    }
}
