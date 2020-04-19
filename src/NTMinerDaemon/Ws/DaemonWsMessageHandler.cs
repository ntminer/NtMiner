using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Ws {
    public static class DaemonWsMessageHandler {
        private static readonly Dictionary<string, Action<Action<WsMessage>, WsMessage>>
            _handlers = new Dictionary<string, Action<Action<WsMessage>, WsMessage>>(StringComparer.OrdinalIgnoreCase);
        public static bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return _handlers.TryGetValue(messageType, out handler);
        }

        static DaemonWsMessageHandler() {
            _handlers.Add(WsMessage.GetConsoleOutLines, (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsNTMinerOpened() && message.TryGetData(out long afterTime)) {
                    RpcRoot.Client.MinerClientService.GetConsoleOutLinesAsync(NTKeyword.Localhost, afterTime, (data, e) => {
                        if (data != null && data.Count != 0) {
                            sendAsync(new WsMessage(message.Id, WsMessage.ConsoleOutLines) {
                                Data = data
                            });
                        }
                    });
                }
            });
            _handlers.Add(WsMessage.GetLocalMessages, (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsNTMinerOpened() && message.TryGetData(out long afterTime)) {
                    RpcRoot.Client.MinerClientService.GetLocalMessagesAsync(NTKeyword.Localhost, afterTime, (data, e) => {
                        if (data != null && data.Count != 0) {
                            sendAsync(new WsMessage(message.Id, WsMessage.LocalMessages) {
                                Data = data
                            });
                        }
                    });
                }
            });
            _handlers.Add(WsMessage.GetDrives, (sendAsync, message) => {
                sendAsync(new WsMessage(message.Id, WsMessage.Drives) {
                    Data = VirtualRoot.DriveSet.AsEnumerable().ToList()
                });
            });
            _handlers.Add(WsMessage.GetLocalIps, (sendAsync, message) => {
                sendAsync(new WsMessage(message.Id, WsMessage.LocalIps) {
                    Data = VirtualRoot.LocalIpSet.AsEnumerable().ToList()
                });
            });
            _handlers.Add(WsMessage.GetOperationResults, (sendAsync, message) => {
                if (message.TryGetData(out long afterTime)) {
                    var data = VirtualRoot.OperationResultSet.Gets(afterTime);
                    if (data != null && data.Count != 0) {
                        sendAsync(new WsMessage(message.Id, WsMessage.OperationResults) {
                            Data = data
                        });
                    }
                }
            });
            _handlers.Add(WsMessage.GetSpeed, (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsNTMinerOpened()) {
                    RpcRoot.Client.MinerClientService.WsGetSpeedAsync((data, ex) => {
                        sendAsync(new WsMessage(message.Id, WsMessage.Speed) {
                            Data = data
                        });
                    });
                }
            });
            _handlers.Add(WsMessage.GetGpuProfilesJson, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    string json = VirtualRoot.DaemonOperation.GetGpuProfilesJson();
                    sendAsync(new WsMessage(message.Id, WsMessage.GpuProfilesJson) {
                        Data = json
                    });
                });
            });
            _handlers.Add(WsMessage.SaveGpuProfilesJson, (sendAsync, message) => {
                if (message.TryGetData(out string json)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SaveGpuProfilesJson(json);
                    });
                }
            });
            _handlers.Add(WsMessage.SetAutoBootStart, (sendAsync, message) => {
                if (message.TryGetData(out SetAutoBootStartRequest data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetAutoBootStart(data.AutoBoot, data.AutoStart);
                    });
                }
            });

            _handlers.Add(WsMessage.EnableRemoteDesktop, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.EnableRemoteDesktop();
                });
            });
            _handlers.Add(WsMessage.BlockWAU, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.BlockWAU();
                });
            });
            _handlers.Add(WsMessage.SetVirtualMemory, (sendAsync, message) => {
                if (message.TryGetData(out Dictionary<string, int> data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetVirtualMemory(data);
                    });
                }
            });
            _handlers.Add(WsMessage.SetLocalIps, (sendAsync, message) => {
                if (message.TryGetData(out List<LocalIpInput> data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetLocalIps(data);
                    });
                }
            });
            _handlers.Add(WsMessage.AtikmdagPatcher, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.AtikmdagPatcher();
                });
            });
            _handlers.Add(WsMessage.SwitchRadeonGpu, (sendAsync, message) => {
                if (message.TryGetData(out bool on)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SwitchRadeonGpu(on);
                    });
                }
            });
            _handlers.Add(WsMessage.RestartWindows, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.RestartWindows();
                });
            });
            _handlers.Add(WsMessage.ShutdownWindows, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.ShutdownWindows();
                });
            });
            _handlers.Add(WsMessage.UpgradeNTMiner, (sendAsync, message) => {
                if (message.TryGetData(out string ntminerFileName)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.UpgradeNTMiner(new UpgradeNTMinerRequest {
                            NTMinerFileName = ntminerFileName
                        });
                    });
                }
            });
            _handlers.Add(WsMessage.StartMine, (sendAsync, message) => {
                if (message.TryGetData(out WorkRequest request)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.StartMine(request);
                    });
                }
            });
            _handlers.Add(WsMessage.StopMine, (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.StopMine();
                });
            });
        }
    }
}
