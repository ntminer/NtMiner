using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public class MinerClientServiceFace {
            public static readonly MinerClientServiceFace Instance = new MinerClientServiceFace();

            private MinerClientServiceFace() {
            }

            public void RestartWindowsAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RestartWindowsRequest innerRequest = new RestartWindowsRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<RestartWindowsRequest> request = new WrapperRequest<RestartWindowsRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "RestartWindows", request, callback);
                });
            }

            public void ShutdownWindowsAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    ShutdownWindowsRequest innerRequest = new ShutdownWindowsRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<ShutdownWindowsRequest> request = new WrapperRequest<ShutdownWindowsRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "ShutdownWindows", request, callback);
                });
            }

            public void OpenNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    OpenNTMinerRequest innerRequest = new OpenNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<OpenNTMinerRequest> request = new WrapperRequest<OpenNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "OpenNTMiner", request, callback);
                });
            }

            public void RestartNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RestartNTMinerRequest innerRequest = new RestartNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<RestartNTMinerRequest> request = new WrapperRequest<RestartNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "RestartNTMiner", request, callback);
                });
            }

            public void UpgradeNTMinerAsync(string clientIp, string ntminerFileName, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    UpgradeNTMinerRequest innerRequest = new UpgradeNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp,
                        NTMinerFileName = ntminerFileName
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<UpgradeNTMinerRequest> request = new WrapperRequest<UpgradeNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "UpgradeNTMiner", request, callback);
                });
            }

            public void CloseNTMinerAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    CloseNTMinerRequest innerRequest = new CloseNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<CloseNTMinerRequest> request = new WrapperRequest<CloseNTMinerRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "CloseNTMiner", request, callback);
                });
            }

            public void StartMineAsync(string clientIp, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    StartMineRequest innerRequest = new StartMineRequest {
                        ClientIp = clientIp,
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<StartMineRequest> request = new WrapperRequest<StartMineRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "StartMine", request, callback);
                });
            }

            public void StopMineAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    StopMineRequest innerRequest = new StopMineRequest {
                        ClientIp = clientIp,
                        LoginName = SingleUser.LoginName
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<StopMineRequest> request = new WrapperRequest<StopMineRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "StopMine", request, callback);
                });
            }

            public void SetClientMinerProfilePropertyAsync(string clientIp, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SetClientMinerProfilePropertyRequest innerRequest = new SetClientMinerProfilePropertyRequest {
                        ClientIp = clientIp,
                        LoginName = SingleUser.LoginName,
                        PropertyName = propertyName,
                        Value = value
                    };
                    innerRequest.SignIt(SingleUser.PasswordSha1Sha1);
                    WrapperRequest<SetClientMinerProfilePropertyRequest> request = new WrapperRequest<SetClientMinerProfilePropertyRequest> {
                        LoginName = SingleUser.LoginName,
                        InnerRequest = innerRequest
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync("MinerClient", "SetClientMinerProfileProperty", request, callback);
                });
            }
        }
    }
}
