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

            public static void RestartWindowsAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RestartWindowsRequest request = new RestartWindowsRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "RestartWindows", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void ShutdownWindowsAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ShutdownWindowsRequest request = new ShutdownWindowsRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "ShutdownWindows", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void OpenNTMinerAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        OpenNTMinerRequest request = new OpenNTMinerRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "OpenNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void RestartNTMinerAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        RestartNTMinerRequest request = new RestartNTMinerRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "RestartNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void UpgradeNTMinerAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        UpgradeNTMinerRequest request = new UpgradeNTMinerRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "UpgradeNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void CloseNTMinerAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        CloseNTMinerRequest request = new CloseNTMinerRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "CloseNTMiner", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void StartMineAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        StartMineRequest request = new StartMineRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "StartMine", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void StopMineAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        StopMineRequest request = new StopMineRequest();
                        ResponseBase response = Request<ResponseBase>("MinerClient", "StopMine", request);
                        callback?.Invoke(response);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public static void SetClientMinerProfilePropertyAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        SetClientMinerProfilePropertyRequest request = new SetClientMinerProfilePropertyRequest();
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
