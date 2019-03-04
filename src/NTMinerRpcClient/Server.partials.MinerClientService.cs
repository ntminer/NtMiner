using NTMiner.Daemon;
using NTMiner.MinerClient;
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
                    RestartWindowsRequest request = new RestartWindowsRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "RestartWindows", request, callback);
                });
            }

            public void ShutdownWindowsAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    ShutdownWindowsRequest request = new ShutdownWindowsRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "ShutdownWindows", request, callback);
                });
            }

            public void OpenNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    OpenNTMinerRequest request = new OpenNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp,
                        WorkId = workId
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "OpenNTMiner", request, callback);
                });
            }

            public void RestartNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RestartNTMinerRequest request = new RestartNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp,
                        WorkId = workId
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "RestartNTMiner", request, callback);
                });
            }

            public void UpgradeNTMinerAsync(string clientIp, string ntminerFileName, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    UpgradeNTMinerRequest request = new UpgradeNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp,
                        NTMinerFileName = ntminerFileName
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "UpgradeNTMiner", request, callback);
                });
            }

            public void CloseNTMinerAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    CloseNTMinerRequest request = new CloseNTMinerRequest {
                        LoginName = SingleUser.LoginName,
                        ClientIp = clientIp
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "CloseNTMiner", request, callback);
                });
            }

            public void StartMineAsync(string clientIp, Guid workId, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    StartMineRequest request = new StartMineRequest {
                        ClientIp = clientIp,
                        LoginName = SingleUser.LoginName,
                        WorkId = workId
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "StartMine", request, callback);
                });
            }

            public void StopMineAsync(string clientIp, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    StopMineRequest request = new StopMineRequest {
                        ClientIp = clientIp,
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "StopMine", request, callback);
                });
            }

            public void SetClientMinerProfilePropertyAsync(string clientIp, string propertyName, object value, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    SetClientMinerProfilePropertyRequest request = new SetClientMinerProfilePropertyRequest {
                        ClientIp = clientIp,
                        LoginName = SingleUser.LoginName,
                        PropertyName = propertyName,
                        Value = value
                    };
                    request.SignIt(SingleUser.PasswordSha1Sha1);
                    RequestAsync("MinerClient", "SetClientMinerProfileProperty", request, callback);
                });
            }
        }
    }
}
