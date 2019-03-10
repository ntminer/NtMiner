using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
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
                    SignatureRequest innerRequest = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest,
                        ClientIp = client.MinerIp
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.RestartWindows), request, callback);
                });
            }

            public void ShutdownWindowsAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SignatureRequest innerRequest = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest,
                        ClientIp = client.MinerIp
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.ShutdownWindows), request, callback);
                });
            }

            public void OpenNTMinerAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    OpenNTMinerRequest innerRequest = new OpenNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<OpenNTMinerRequest> request = new WrapperRequest<OpenNTMinerRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest,
                        ClientIp = client.MinerIp
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.OpenNTMiner), request, callback);
                });
            }

            public void RestartNTMinerAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    WorkRequest innerRequest = new WorkRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<WorkRequest> request = new WrapperRequest<WorkRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest,
                        ClientIp = client.MinerIp
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.RestartNTMiner), request, callback);
                });
            }

            public void UpgradeNTMinerAsync(IClientData client, string ntminerFileName, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    UpgradeNTMinerRequest innerRequest = new UpgradeNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        NTMinerFileName = ntminerFileName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<UpgradeNTMinerRequest> request = new WrapperRequest<UpgradeNTMinerRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest,
                        ClientIp = client.MinerIp
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.UpgradeNTMiner), request, callback);
                });
            }

            public void CloseNTMinerAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SignatureRequest innerRequest = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest,
                        ClientIp = client.MinerIp
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.CloseNTMiner), request, callback);
                });
            }

            public void StartMineAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    WorkRequest innerRequest = new WorkRequest {
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<WorkRequest> request = new WrapperRequest<WorkRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.StartMine), request, callback);
                });
            }

            public void StopMineAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SignatureRequest innerRequest = new SignatureRequest {
                        LoginName = SingleUser.LoginName
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.StopMine), request, callback);
                });
            }

            public void SetClientMinerProfilePropertyAsync(IClientData client, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SetClientMinerProfilePropertyRequest innerRequest = new SetClientMinerProfilePropertyRequest {
                        LoginName = SingleUser.LoginName,
                        PropertyName = propertyName,
                        Value = value
                    };
                    innerRequest.SignIt(SingleUser.GetRemotePassword(client.GetId()));
                    WrapperRequest<SetClientMinerProfilePropertyRequest> request = new WrapperRequest<SetClientMinerProfilePropertyRequest> {
                        ClientId = client.GetId(),
                        LoginName = SingleUser.LoginName,
                        ClientIp = client.MinerIp,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(s_controllerName, nameof(IWrapperMinerClientController.SetClientMinerProfileProperty), request, callback);
                });
            }
        }
    }
}
