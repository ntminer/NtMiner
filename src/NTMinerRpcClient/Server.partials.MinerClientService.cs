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

            public void RestartWindowsAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RestartWindowsRequest request = new RestartWindowsRequest {
                            LoginName = SingleUser.LoginName,
                            ClientIp = clientIp
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "RestartWindows", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void ShutdownWindowsAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ShutdownWindowsRequest request = new ShutdownWindowsRequest {
                            LoginName = SingleUser.LoginName,
                            ClientIp = clientIp
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "ShutdownWindows", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void OpenNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        OpenNTMinerRequest request = new OpenNTMinerRequest {
                            LoginName = SingleUser.LoginName,
                            ClientIp = clientIp,
                            WorkId = workId
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "OpenNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void RestartNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RestartNTMinerRequest request = new RestartNTMinerRequest {
                            LoginName = SingleUser.LoginName,
                            ClientIp = clientIp,
                            WorkId = workId
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "RestartNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void UpgradeNTMinerAsync(string clientIp, string ntminerFileName, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpgradeNTMinerRequest request = new UpgradeNTMinerRequest {
                            LoginName = SingleUser.LoginName,
                            ClientIp = clientIp,
                            NTMinerFileName = ntminerFileName
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "UpgradeNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void CloseNTMinerAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        CloseNTMinerRequest request = new CloseNTMinerRequest {
                            LoginName = SingleUser.LoginName,
                            ClientIp = clientIp
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "CloseNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void StartMineAsync(string clientIp, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        StartMineRequest request = new StartMineRequest {
                            ClientIp = clientIp,
                            LoginName = SingleUser.LoginName,
                            WorkId = workId
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "StartMine", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void StopMineAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        StopMineRequest request = new StopMineRequest {
                            ClientIp = clientIp,
                            LoginName = SingleUser.LoginName
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "StopMine", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void SetClientMinerProfilePropertyAsync(string clientIp, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetClientMinerProfilePropertyRequest request = new SetClientMinerProfilePropertyRequest {
                            ClientIp = clientIp,
                            LoginName = SingleUser.LoginName,
                            PropertyName = propertyName,
                            Value = value
                        };
                        request.SignIt(SingleUser.PasswordSha1Sha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "SetClientMinerProfileProperty", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
        }
    }
}
