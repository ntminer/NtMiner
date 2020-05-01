using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.Services.Client {
    public class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private readonly string _controllerName = RpcRoot.GetControllerName<IMinerClientController>();
        private MinerClientService() {
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void ShowMainWindowAsync(Action<bool, Exception> callback) {
            RpcRoot.PostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.ShowMainWindow), callback);
        }

        /// <summary>
        /// 本机同步网络调用
        /// </summary>
        public void CloseNTMinerAsync(Action callback) {
            string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
            if (string.IsNullOrEmpty(location) || !File.Exists(location)) {
                callback?.Invoke();
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(location);
            if (Process.GetProcessesByName(processName).Length == 0) {
                callback?.Invoke();
                return;
            }
            RpcRoot.PostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.CloseNTMiner), new object { }, (ResponseBase response, Exception e) => {
                if (!response.IsSuccess()) {
                    try {
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                    }
                    catch (Exception ex) {
                        Logger.ErrorDebugLine(ex);
                    }
                }
                callback?.Invoke();
            }, timeountMilliseconds: 2000);
        }

        public void GetSpeedAsync(IMinerData client, Action<SpeedData, Exception> callback) {
            RpcRoot.GetAsync(client.GetLocalIp(), NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.GetSpeed), null, callback, timeountMilliseconds: 3000);
        }

        public void WsGetSpeedAsync(Action<SpeedData, Exception> callback) {
            RpcRoot.GetAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.WsGetSpeed), null, callback, timeountMilliseconds: 3000);
        }

        public void WsGetSpeedGZipedAsync(Action<byte[], Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        client.Timeout = TimeSpan.FromMilliseconds(3000);
                        Task<HttpResponseMessage> message = client.GetAsync($"http://{NTKeyword.Localhost}:{NTKeyword.MinerClientPort.ToString()}/api/{_controllerName}/{nameof(IMinerClientController<HttpResponseMessage>.WsGetSpeedGZiped)}");
                        message.Result.Content.ReadAsByteArrayAsync().ContinueWith(t => {
                            callback?.Invoke(t.Result, null);
                        });
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(new byte[0], e);
                }
            });
        }

        public void GetConsoleOutLinesAsync(string clientIp, long afterTime, Action<List<ConsoleOutLine>, Exception> callback) {
            RpcRoot.GetAsync(clientIp, NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.GetConsoleOutLines), new Dictionary<string, string> {
                {"afterTime",afterTime.ToString() }
            }, callback, timeountMilliseconds: 3000);
        }

        public void GetLocalMessagesAsync(string clientIp, long afterTime, Action<List<LocalMessageDto>, Exception> callback) {
            RpcRoot.GetAsync(clientIp, NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.GetLocalMessages), new Dictionary<string, string> {
                {"afterTime",afterTime.ToString() }
            }, callback, timeountMilliseconds: 3000);
        }
    }
}
