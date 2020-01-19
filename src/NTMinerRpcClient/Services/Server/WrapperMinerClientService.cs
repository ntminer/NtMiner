using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;

namespace NTMiner.Services.Server {
    public class WrapperMinerClientService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IWrapperMinerClientController>();

        public WrapperMinerClientService() {
        }

        public void RestartWindowsAsync(IMinerData client, Action<ResponseBase, Exception> callback) {
            SignRequest innerRequest = new SignRequest {
            };
            WrapperRequest<SignRequest> request = new WrapperRequest<SignRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                InnerRequest = innerRequest,
                ClientIp = client.MinerIp
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.RestartWindows), request, request, callback);
        }

        public void ShutdownWindowsAsync(IMinerData client, Action<ResponseBase, Exception> callback) {
            SignRequest innerRequest = new SignRequest {
            };
            WrapperRequest<SignRequest> request = new WrapperRequest<SignRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                InnerRequest = innerRequest,
                ClientIp = client.MinerIp
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.ShutdownWindows), request, request, callback);
        }

        // ReSharper disable once InconsistentNaming
        public void RestartNTMinerAsync(IMinerData client, Action<ResponseBase, Exception> callback) {
            WorkRequest innerRequest = new WorkRequest {
                WorkId = client.WorkId
            };
            WrapperRequest<WorkRequest> request = new WrapperRequest<WorkRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                InnerRequest = innerRequest,
                ClientIp = client.MinerIp
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.RestartNTMiner), request, request, callback);
        }

        // ReSharper disable once InconsistentNaming
        public void UpgradeNTMinerAsync(IMinerData client, string ntminerFileName, Action<ResponseBase, Exception> callback) {
            UpgradeNTMinerRequest innerRequest = new UpgradeNTMinerRequest {
                NTMinerFileName = ntminerFileName
            };
            WrapperRequest<UpgradeNTMinerRequest> request = new WrapperRequest<UpgradeNTMinerRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                InnerRequest = innerRequest,
                ClientIp = client.MinerIp
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.UpgradeNTMiner), request, request, callback);
        }

        public void StartMineAsync(IMinerData client, Guid workId, Action<ResponseBase, Exception> callback) {
            WorkRequest innerRequest = new WorkRequest {
                WorkId = workId
            };
            WrapperRequest<WorkRequest> request = new WrapperRequest<WorkRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                ClientIp = client.MinerIp,
                InnerRequest = innerRequest
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.StartMine), request, request, callback);
        }

        public void StopMineAsync(IMinerData client, Action<ResponseBase, Exception> callback) {
            SignRequest innerRequest = new SignRequest {
            };
            WrapperRequest<SignRequest> request = new WrapperRequest<SignRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                ClientIp = client.MinerIp,
                InnerRequest = innerRequest
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.StopMine), request, request, callback);
        }

        public void SetClientMinerProfilePropertyAsync(IMinerData client, string propertyName, object value, Action<ResponseBase, Exception> callback) {
            SetClientMinerProfilePropertyRequest innerRequest = new SetClientMinerProfilePropertyRequest {
                PropertyName = propertyName,
                Value = value
            };
            WrapperRequest<SetClientMinerProfilePropertyRequest> request = new WrapperRequest<SetClientMinerProfilePropertyRequest> {
                ObjectId = client.Id,
                ClientId = client.ClientId,
                ClientIp = client.MinerIp,
                InnerRequest = innerRequest
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IWrapperMinerClientController.SetClientMinerProfileProperty), request, request, callback);
        }
    }
}
