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

            public static void RestartWindowsAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RestartWindowsRequest request = new RestartWindowsRequest {
                            ClientIp = clientIp
                        };
                        ResponseBase response = Request<ResponseBase>("MinerClient", "RestartWindows", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void ShutdownWindowsAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ShutdownWindowsRequest request = new ShutdownWindowsRequest {
                            ClientIp = clientIp
                        };
                        ResponseBase response = Request<ResponseBase>("MinerClient", "ShutdownWindows", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void OpenNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        OpenNTMinerRequest request = new OpenNTMinerRequest {
                            ClientIp = clientIp,
                            WorkId = workId
                        };
                        ResponseBase response = Request<ResponseBase>("MinerClient", "OpenNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void RestartNTMinerAsync(string clientIp, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RestartNTMinerRequest request = new RestartNTMinerRequest {
                            ClientIp = clientIp,
                            WorkId = workId
                        };
                        ResponseBase response = Request<ResponseBase>("MinerClient", "RestartNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void UpgradeNTMinerAsync(string clientIp, string ntminerFileName, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpgradeNTMinerRequest request = new UpgradeNTMinerRequest {
                            ClientIp = clientIp,
                            NTMinerFileName = ntminerFileName
                        };
                        ResponseBase response = Request<ResponseBase>("MinerClient", "UpgradeNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void CloseNTMinerAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        CloseNTMinerRequest request = new CloseNTMinerRequest {
                            ClientIp = clientIp
                        };
                        ResponseBase response = Request<ResponseBase>("MinerClient", "CloseNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void StartMineAsync(string clientIp, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        StartMineRequest request = new StartMineRequest {
                            ClientIp = clientIp,
                            LoginName = LoginName,
                            WorkId = workId
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "StartMine", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void StopMineAsync(string clientIp, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        StopMineRequest request = new StopMineRequest {
                            ClientIp = clientIp,
                            LoginName = LoginName
                        };
                        request.SignIt(PasswordSha1);
                        ResponseBase response = Request<ResponseBase>("MinerClient", "StopMine", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void SetClientMinerProfilePropertyAsync(string clientIp, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetClientMinerProfilePropertyRequest request = new SetClientMinerProfilePropertyRequest {
                            ClientIp = clientIp,
                            LoginName = LoginName,
                            PropertyName = propertyName,
                            Value = value
                        };
                        request.SignIt(PasswordSha1);
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
