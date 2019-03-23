using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;

namespace NTMiner {
    public partial class Server {
        public class WrapperMinerClientServiceFace {
            public static readonly WrapperMinerClientServiceFace Instance = new WrapperMinerClientServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IWrapperMinerClientController>();

            private WrapperMinerClientServiceFace() {
            }

            public void RestartWindowsAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                SignatureRequest innerRequest = new SignatureRequest {
                    LoginName = SingleUser.LoginName
                };
                innerRequest.SignIt(SingleUser.GetRemotePassword(client.ClientId));
                WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    InnerRequest = innerRequest,
                    ClientIp = client.MinerIp
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.RestartWindows), request, callback);
            }

            public void ShutdownWindowsAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                SignatureRequest innerRequest = new SignatureRequest {
                    LoginName = SingleUser.LoginName
                };
                innerRequest.SignIt(SingleUser.GetRemotePassword(client.ClientId));
                WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    InnerRequest = innerRequest,
                    ClientIp = client.MinerIp
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.ShutdownWindows), request, callback);
            }

            // ReSharper disable once InconsistentNaming
            public void RestartNTMinerAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                WorkRequest innerRequest = new WorkRequest {
                    WorkId = client.WorkId
                };
                WrapperRequest<WorkRequest> request = new WrapperRequest<WorkRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    InnerRequest = innerRequest,
                    ClientIp = client.MinerIp
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.RestartNTMiner), request, callback);
            }

            // ReSharper disable once InconsistentNaming
            public void UpgradeNTMinerAsync(IClientData client, string ntminerFileName, Action<ResponseBase, Exception> callback) {
                UpgradeNTMinerRequest innerRequest = new UpgradeNTMinerRequest {
                    LoginName = SingleUser.LoginName,
                    NTMinerFileName = ntminerFileName
                };
                innerRequest.SignIt(SingleUser.GetRemotePassword(client.ClientId));
                WrapperRequest<UpgradeNTMinerRequest> request = new WrapperRequest<UpgradeNTMinerRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    InnerRequest = innerRequest,
                    ClientIp = client.MinerIp
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.UpgradeNTMiner), request, callback);
            }

            public void StartMineAsync(IClientData client, Guid workId, Action<ResponseBase, Exception> callback) {
                WorkRequest innerRequest = new WorkRequest {
                    WorkId = workId
                };
                WrapperRequest<WorkRequest> request = new WrapperRequest<WorkRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    ClientIp = client.MinerIp,
                    InnerRequest = innerRequest
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.StartMine), request, callback);
            }

            public void StopMineAsync(IClientData client, Action<ResponseBase, Exception> callback) {
                SignatureRequest innerRequest = new SignatureRequest {
                    LoginName = SingleUser.LoginName
                };
                innerRequest.SignIt(SingleUser.GetRemotePassword(client.ClientId));
                WrapperRequest<SignatureRequest> request = new WrapperRequest<SignatureRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    ClientIp = client.MinerIp,
                    InnerRequest = innerRequest
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.StopMine), request, callback);
            }

            public void SetClientMinerProfilePropertyAsync(IClientData client, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                SetClientMinerProfilePropertyRequest innerRequest = new SetClientMinerProfilePropertyRequest {
                    LoginName = SingleUser.LoginName,
                    PropertyName = propertyName,
                    Value = value
                };
                innerRequest.SignIt(SingleUser.GetRemotePassword(client.ClientId));
                WrapperRequest<SetClientMinerProfilePropertyRequest> request = new WrapperRequest<SetClientMinerProfilePropertyRequest> {
                    ObjectId = client.GetId(),
                    ClientId = client.ClientId,
                    LoginName = SingleUser.LoginName,
                    ClientIp = client.MinerIp,
                    InnerRequest = innerRequest
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IWrapperMinerClientController.SetClientMinerProfileProperty), request, callback);
            }
        }
    }
}
