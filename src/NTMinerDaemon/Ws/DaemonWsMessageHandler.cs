using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTMiner.Ws {
    public static class DaemonWsMessageHandler {
        private static readonly Dictionary<string, Action<Action<WsMessage>, WsMessage>> _handlers = new Dictionary<string, Action<Action<WsMessage>, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
            [WsMessage.GetConsoleOutLines] = (sendAsync, message) => {
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
            },
            [WsMessage.GetLocalMessages] = (sendAsync, message) => {
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
            },
            [WsMessage.GetDrives] = (sendAsync, message) => {
                sendAsync(new WsMessage(message.Id, WsMessage.Drives) {
                    Data = VirtualRoot.DriveSet.AsEnumerable().ToList()
                });
            },
            [WsMessage.GetLocalIps] = (sendAsync, message) => {
                sendAsync(new WsMessage(message.Id, WsMessage.LocalIps) {
                    Data = VirtualRoot.LocalIpSet.AsEnumerable().ToList()
                });
            },
            [WsMessage.GetOperationResults] = (sendAsync, message) => {
                if (message.TryGetData(out long afterTime)) {
                    var data = VirtualRoot.OperationResultSet.Gets(afterTime);
                    if (data != null && data.Count != 0) {
                        sendAsync(new WsMessage(message.Id, WsMessage.OperationResults) {
                            Data = data
                        });
                    }
                }
            },
            [WsMessage.GetSpeed] = (sendAsync, message) => {
                // 如果进程不存在就不用Rpc了
                if (VirtualRoot.DaemonOperation.IsNTMinerOpened()) {
                    RpcRoot.Client.MinerClientService.WsGetSpeedAsync((data, ex) => {
                        sendAsync(new WsMessage(message.Id, WsMessage.Speed) {
                            Data = data
                        });
                    });
                }
            },
            [WsMessage.GetSelfWorkLocalJson] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    string json = VirtualRoot.DaemonOperation.GetSelfWorkLocalJson();
                    sendAsync(new WsMessage(message.Id, WsMessage.SelfWorkLocalJson) {
                        Data = json
                    });
                });
            },
            [WsMessage.SaveSelfWorkLocalJson] = (sendAsync, message) => {
                if (message.TryGetData(out WorkRequest workRequest)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SaveSelfWorkLocalJson(workRequest);
                    });
                }
            },
            [WsMessage.GetGpuProfilesJson] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    string json = VirtualRoot.DaemonOperation.GetGpuProfilesJson();
                    sendAsync(new WsMessage(message.Id, WsMessage.GpuProfilesJson) {
                        Data = json
                    });
                });
            },
            [WsMessage.SaveGpuProfilesJson] = (sendAsync, message) => {
                if (message.TryGetData(out string json)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SaveGpuProfilesJson(json);
                    });
                }
            },
            [WsMessage.SetAutoBootStart] = (sendAsync, message) => {
                if (message.TryGetData(out SetAutoBootStartRequest data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetAutoBootStart(data.AutoBoot, data.AutoStart);
                    });
                }
            },
            [WsMessage.EnableRemoteDesktop] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.EnableRemoteDesktop();
                });
            },
            [WsMessage.BlockWAU] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.BlockWAU();
                });
            },
            [WsMessage.SetVirtualMemory] = (sendAsync, message) => {
                if (message.TryGetData(out Dictionary<string, int> data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetVirtualMemory(data);
                    });
                }
            },
            [WsMessage.SetLocalIps] = (sendAsync, message) => {
                if (message.TryGetData(out List<LocalIpInput> data)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SetLocalIps(data);
                    });
                }
            },
            [WsMessage.AtikmdagPatcher] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.AtikmdagPatcher();
                });
            },
            [WsMessage.SwitchRadeonGpu] = (sendAsync, message) => {
                if (message.TryGetData(out bool on)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.SwitchRadeonGpu(on);
                    });
                }
            },
            [WsMessage.RestartWindows] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.RestartWindows();
                });
            },
            [WsMessage.ShutdownWindows] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.ShutdownWindows();
                });
            },
            [WsMessage.UpgradeNTMiner] = (sendAsync, message) => {
                if (message.TryGetData(out string ntminerFileName)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.UpgradeNTMiner(new UpgradeNTMinerRequest {
                            NTMinerFileName = ntminerFileName
                        });
                    });
                }
            },
            [WsMessage.StartMine] = (sendAsync, message) => {
                if (message.TryGetData(out WorkRequest request)) {
                    Task.Factory.StartNew(() => {
                        VirtualRoot.DaemonOperation.StartMine(request);
                    });
                }
            },
            [WsMessage.StopMine] = (sendAsync, message) => {
                Task.Factory.StartNew(() => {
                    VirtualRoot.DaemonOperation.StopMine();
                });
            }
        };

        public static bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler) {
            return _handlers.TryGetValue(messageType, out handler);
        }
    }
}
